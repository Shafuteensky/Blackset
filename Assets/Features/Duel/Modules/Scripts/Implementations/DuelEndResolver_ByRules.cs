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
                    // TODO Тай-брейк по вторичному критерию: если из 4 битв оба выиграли 2 раза, то засчитывать победу по набранным очкам
                    // (добавить трекер очков: + за неизрасходованные дайсы/расходники)
                    if (IsSomeoneWonMoreThanHalfFights(context, out string winnerId))
                        result.WinnerId = winnerId;
                    break;
                }
                // Необработанные случаи
                default:
                {
                    ServiceDebug.LogError($"Необработанный случай политики окончания дуэли ({activeDuelWinPolicy})");
                    break;
                }
            }

            // Есть победитель
            if (!string.IsNullOrEmpty(result.WinnerId))
            {
                result.IsDuelEnded = true;
                bool IsWinnerPlayer = context.Participants[result.WinnerId].IsPlayer;
                result.Winner = IsWinnerPlayer ? FightWinner.Player : FightWinner.Opponent;
                return result;
            }
            // Победителя нет — ничья
            if (IsLastFightDone(context))
            {
                result.IsDuelEnded = true;
                result.Winner = FightWinner.None;
                result.WinnerId = string.Empty;
            }

            return result;
        }

        #region  Проверки по правилам

        /// <summary>
        /// Участник победил в более чем половине максимума боев дуэли
        /// </summary>
        private bool IsSomeoneWonMoreThanHalfFights(DuelContext context, out string winnerId)
        {
            winnerId = string.Empty;
            
            foreach (DuelParticipantState participant in context.Participants.Values)
            {
                if (IsMoreThanHalfWon(context, participant.FightsWon))
                {
                    winnerId = participant.ParticipantId;
                    return true;
                }
            }
            
            return false;
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
        private bool IsLastFightDone(DuelContext context) => context.Progress.FightNumber == context.Rules.MaxFightsPerDuel;
        
        #endregion
    }
}