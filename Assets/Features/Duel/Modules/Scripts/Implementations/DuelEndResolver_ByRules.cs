using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Duel.Rules;
using Extensions.Log;
using Features.Duel.Context;
using Features.Duel.Data.FightEnd;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Стандартный резолвер окончания дуэли (по правилам дуэли)
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(DuelEndResolver_ByRules),
        menuName = "Blackset/Duel/Modules/" + nameof(DuelEndResolver_ByRules))]
    public class DuelEndResolver_ByRules : BaseDuelModule, IDuelEndResolver
    {
        public DuelEndResult Evaluate(DuelContext context)
        {
            ServiceGuard.NotNull(context, nameof(context));
            
            DuelEndResult clearResult = new(false, string.Empty, FightWinner.None);
            DuelEndResult resolvedResult = EvaluateByRules(context, clearResult);
            
            return resolvedResult;
        }

        private DuelEndResult EvaluateByRules(DuelContext context, DuelEndResult result)
        {
            DuelWinPolicy activeDuelWinPolicy = context.Rules.DuelWinPolicy;
            switch (activeDuelWinPolicy)
            {
                case DuelWinPolicy.WinMostFights:
                {
                    if (IsSomeoneWonMoreThanHalfFights(context, out string winnerId))
                    {
                        result.WinnerId = winnerId;
                    }

                    break;
                }
                default:
                {
                    ServiceDebug.LogError($"Необработанный случай политики окончания дуэли ({activeDuelWinPolicy})");
                    break;
                }
            }

            if (!string.IsNullOrEmpty(result.WinnerId))
            {
                result.IsDuelEnded.Value = true;
                bool isWinnerPlayer = context.Participants[result.WinnerId].IsPlayer;
                result.Winner.Value = isWinnerPlayer ? FightWinner.Player : FightWinner.Opponent;
                return result;
            }

            if (IsLastFightDone(context))
            {
                result.IsDuelEnded.Value = true;

                if (TryResolveWinnerByFightsOrScore(context, out string winnerId))
                {
                    result.WinnerId = winnerId;

                    bool isWinnerPlayer = context.Participants[winnerId].IsPlayer;
                    result.Winner.Value = isWinnerPlayer ? FightWinner.Player : FightWinner.Opponent;
                }
                else
                {
                    result.Winner.Value = FightWinner.None;
                    result.WinnerId = string.Empty;
                }
            }

            return result;
        }

        #region Проверки по правилам

        /// <summary>
        /// Участник победил в более чем половине максимума боев дуэли
        /// </summary>
        private bool IsSomeoneWonMoreThanHalfFights(DuelContext context, out string winnerId)
        {
            winnerId = string.Empty;
            
            foreach (DuelParticipantState participant in context.Participants.Values)
            {
                if (IsMoreThanHalfWon(context, participant.FightsWon.Value))
                {
                    winnerId = participant.ParticipantId;
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Определить победителя по количеству выигранных боев или по очкам дуэли
        /// </summary>
        private bool TryResolveWinnerByFightsOrScore(DuelContext context, out string winnerId)
        {
            winnerId = string.Empty;

            DuelParticipantState bestParticipant = null;
            bool hasTieByFights = false;

            foreach (DuelParticipantState participant in context.Participants.Values)
            {
                if (bestParticipant == null)
                {
                    bestParticipant = participant;
                    hasTieByFights = false;
                    continue;
                }

                if (participant.FightsWon.Value > bestParticipant.FightsWon.Value)
                {
                    bestParticipant = participant;
                    hasTieByFights = false;
                    continue;
                }

                if (participant.FightsWon.Value == bestParticipant.FightsWon.Value)
                {
                    hasTieByFights = true;
                }
            }

            if (bestParticipant == null) return false;

            if (hasTieByFights == false)
            {
                winnerId = bestParticipant.ParticipantId;
                return true;
            }

            DuelParticipantState bestByScore = null;
            bool hasTieByScore = false;

            foreach (DuelParticipantState participant in context.Participants.Values)
            {
                if (bestByScore == null)
                {
                    bestByScore = participant;
                    hasTieByScore = false;
                    continue;
                }

                if (participant.DuelScore.Value > bestByScore.DuelScore.Value)
                {
                    bestByScore = participant;
                    hasTieByScore = false;
                    continue;
                }

                if (participant.DuelScore.Value == bestByScore.DuelScore.Value) hasTieByScore = true;
            }

            if (bestByScore == null || hasTieByScore) return false;

            winnerId = bestByScore.ParticipantId;
            return true;
        }
        
        /// <summary>
        /// Выиграно более половины максимума боев
        /// </summary>
        private bool IsMoreThanHalfWon(DuelContext context, int actual)
        {
            int maximumFights = context.Rules.MaxFightsPerDuel;
            if (maximumFights <= 0) return false;

            return actual * 2 > maximumFights;
        }
        
        /// <summary>
        /// Максимум возможных по правилам боев завершены
        /// </summary>
        private bool IsLastFightDone(DuelContext context) => context.Progress.FightNumber.Value == context.Rules.MaxFightsPerDuel;
        
        #endregion
    }
}