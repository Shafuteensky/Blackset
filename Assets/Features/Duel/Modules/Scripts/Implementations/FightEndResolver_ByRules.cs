using System;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Duel.Rules;
using Extensions.Log;
using Features.Duel.Data.FightEnd;
using UnityEngine;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Стандартный резолвер окончания битвы дуэли (по правилам дуэли)
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(FightEndResolver_ByRules),
        menuName = "Blackset/Duel/Modules/" + nameof(FightEndResolver_ByRules))]
    public class FightEndResolver_ByRules : BaseDuelModule, IFightEndResolver
    {
        public FightEndResult Evaluate(DuelContext context)
        {
            ServiceGuard.NotNull(context, nameof(context));

            FightEndResult clearResult = new(false, String.Empty, FightWinner.None, FightEndReason.None);
            FightEndResult resolvedResult = EvaluateByRules(context, clearResult);

            return resolvedResult;
        }

        private FightEndResult EvaluateByRules(DuelContext context, FightEndResult result)
        {
            // Кандидат победителя — актуален только при сравнительных политиках, без автопобеды
            string candidateWinnerId = string.Empty;
            bool isCandidateTie = false;

            int bestAbsDelta = int.MaxValue;
            int bestUnderTargetScore = int.MinValue;
            int minScore = int.MaxValue;

            foreach (DuelParticipantState participant in context.Participants.Values)
            {
                // Автопроигрышь — немедленно завершаем бой, победитель — противоположная сторона
                if (IsSomeoneLostByRules(context, participant, ref minScore, out string currentLoserId))
                    return ResolveByLoser(context, result, currentLoserId);

                // Автопобеда — немедленно завершаем бой
                if (IsSomeoneWonByRules(context, participant, ref bestAbsDelta, ref bestUnderTargetScore,
                        ref candidateWinnerId, ref isCandidateTie))
                    return ResolveByWinner(context, result, candidateWinnerId);
            }

            // Нет автопобеды/автопроигрыша — бой продолжается до последнего хода
            if (!IsLastTurnDone(context))
                return result;

            // Последний ход сыгран — определяем итог по кандидатам
            if (!string.IsNullOrEmpty(candidateWinnerId) && !isCandidateTie)
                return ResolveByWinner(context, result, candidateWinnerId);

            // Ничья: нет кандидата или ничья по дельте
            result.IsFightEnded = true;
            result.Winner = FightWinner.None;
            result.WinnerId = string.Empty;
            result.Reason = FightEndReason.Draw;
            return result;
        }

        #region Проверки по правилам

        /// <summary>
        /// Максимум возможных по правилам ходов завершены
        /// </summary>
        private bool IsLastTurnDone(DuelContext context) =>
            context.Progress.ThrowNumber == context.Rules.MaxThrowsPerFight;

        /// <summary>
        /// Проверяет участника на победу.
        /// Возвращает true только при автопобеде (exact / bust) — бой нужно завершить немедленно.
        /// При сравнительных политиках обновляет candidateWinnerId и isCandidateTie,
        /// но возвращает false — бой продолжается до конца ходов.
        /// </summary>
        private bool IsSomeoneWonByRules(
            DuelContext context,
            DuelParticipantState participant,
            ref int bestAbsDelta,
            ref int bestUnderTargetScore,
            ref string candidateWinnerId,
            ref bool isCandidateTie)
        {
            FightWinPolicy activeFightWinPolicy = context.Rules.FightWinPolicy;

            int score = participant.FightState.Score;
            int target = context.TargetValue.TargetValue;

            switch (activeFightWinPolicy)
            {
                case FightWinPolicy.ExactOrClosest:
                {
                    // Точное попадание в ЦЗ — автопобеда
                    if (score == target)
                    {
                        candidateWinnerId = participant.ParticipantId;
                        return true;
                    }

                    // Ближайший снизу — обновляем кандидата, без завершения боя
                    if (score <= target && score > bestUnderTargetScore)
                    {
                        bestUnderTargetScore = score;
                        candidateWinnerId = participant.ParticipantId;
                        isCandidateTie = false;
                    }

                    break;
                }
                
                case FightWinPolicy.ClosestOnly:
                {
                    // Абсолютная близость к ЦЗ — обновляем кандидата, без завершения боя
                    int absDelta = Math.Abs(target - score);

                    if (absDelta < bestAbsDelta)
                    {
                        bestAbsDelta = absDelta;
                        candidateWinnerId = participant.ParticipantId;
                        isCandidateTie = false;
                    }
                    else if (absDelta == bestAbsDelta)
                    {
                        // Два участника одинаково близко к ЦЗ — ничья
                        isCandidateTie = true;
                        candidateWinnerId = string.Empty;
                    }

                    break;
                }
                
                case FightWinPolicy.Bust:
                {
                    // Пробитие ЦЗ — автопобеда
                    if (score >= target)
                    {
                        candidateWinnerId = participant.ParticipantId;
                        return true;
                    }

                    break;
                }
                
                default:
                {
                    ServiceDebug.LogError(
                        $"Необработанный случай политики окончания битвы при победе ({activeFightWinPolicy})");
                    break;
                }
            }

            return false; // Не автопобеда
        }

        private bool IsSomeoneLostByRules(
            DuelContext context,
            DuelParticipantState participant,
            ref int minScore,
            out string loserId)
        {
            FightLossPolicy activeFightLossPolicy = context.Rules.FightLossPolicy;
            loserId = string.Empty;

            int score = participant.FightState.Score;
            int target = context.TargetValue.TargetValue;

            switch (activeFightLossPolicy)
            {
                case FightLossPolicy.LessOrBust:
                {
                    // Пробитие ЦЗ — автопроигрышь
                    if (score > target)
                    {
                        loserId = participant.ParticipantId;
                        return true;
                    }

                    // Участник слабее по счёту — обновляем минимум
                    if (score < minScore)
                    {
                        minScore = score;
                        loserId = participant.ParticipantId;
                    }

                    break;
                }
                
                case FightLossPolicy.Less:
                {
                    // Участник слабее по счёту — обновляем минимум
                    if (score < minScore)
                    {
                        minScore = score;
                        loserId = participant.ParticipantId;
                    }

                    break;
                }
                
                default:
                {
                    ServiceDebug.LogError(
                        $"Необработанный случай политики окончания битвы при поражении ({activeFightLossPolicy})");
                    break;
                }
            }

            return false;
        }

        #endregion

        #region Хелперы финализации результата

        private FightEndResult ResolveByWinner(DuelContext context, FightEndResult result, string winnerId)
        {
            result.IsFightEnded = true;
            result.WinnerId = winnerId;
            bool isWinnerPlayer = context.Participants[winnerId].IsPlayer;
            result.Winner = isWinnerPlayer ? FightWinner.Player : FightWinner.Opponent;
            result.Reason = isWinnerPlayer ? FightEndReason.PlayerWon : FightEndReason.OpponentWon;
            return result;
        }

        private FightEndResult ResolveByLoser(DuelContext context, FightEndResult result, string loserId)
        {
            result.IsFightEnded = true;
            bool isLoserPlayer = context.Participants[loserId].IsPlayer;

            // Победитель — первый участник, который не является проигравшим
            foreach (DuelParticipantState participant in context.Participants.Values)
            {
                if (participant.ParticipantId == loserId)
                    continue;

                result.WinnerId = participant.ParticipantId;
                result.Winner = participant.IsPlayer ? FightWinner.Player : FightWinner.Opponent;
                break;
            }

            if (string.IsNullOrEmpty(result.WinnerId))
            {
                ServiceDebug.LogError($"Автопроигрыш по id='{loserId}', но победитель не определён (кол-во участников={context.Participants.Count})");
                result.Winner = FightWinner.None;
                result.WinnerId = string.Empty;
                result.Reason = FightEndReason.Draw;
                return result;
            }

            result.Reason = isLoserPlayer ? FightEndReason.PlayerLost : FightEndReason.OpponentLost;
            return result;
        }

        #endregion
    }
}