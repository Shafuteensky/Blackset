using System.Collections.Generic;
using Blackset.Duel.Context;
using Blackset.Duel.History;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.Duel.Rolls;
using Blackset.Effects;
using Extensions.Helpers;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Duel.Snapshots
{
    /// <summary>
    /// Перенос данных текущего броска из снапшота в "истину" дуэли
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(SnapshotCommiter_Default),
        menuName = "Blackset/Duel/Modules/" + nameof(SnapshotCommiter_Default))]
    public class SnapshotCommiter_Default : BaseDuelModule, ISnapshotCommiter
    {
        /// <summary>
        /// Коммит снапшота
        /// </summary>
        /// <param name="resolvedSnapshot">Данные снапшота текущего броска</param>
        /// <param name="context">Данные дуэли</param>
        public void Commit(TurnSnapshot resolvedSnapshot, DuelContext context)
        {
            if (context.Participants.Count == 0)
            {
                ServiceDebug.LogError($"Участники не зарегестрированны в {nameof(TurnSnapshot)}, снапшот не применен");
                return;
            }

            ApplyScores(resolvedSnapshot, context);
            ApplyRollResults(resolvedSnapshot, context);
            ApplyUsage(resolvedSnapshot, context);
            AppendHistory(resolvedSnapshot, context);
        }

        #region Применение данных снапшота к фактическим данным дуэли

        /// <summary>
        /// Применяет финальные счета участников из снапшота
        /// </summary>
        private void ApplyScores(TurnSnapshot snapshot, DuelContext context)
        {
            foreach (KeyValuePair<string, DuelParticipantState> pair in context.Participants)
            {
                if (!snapshot.ParticipantScores.TryGetValue(pair.Key, out int snapshotScore))
                {
                    ServiceDebug.LogError($"Счет участника '{pair.Key}' отсутствует в снапшоте, пропущен");
                    continue;
                }
                
                pair.Value.FightState.AddScore(snapshotScore);
            }
        }
        
        /// <summary>
        /// Применяет результаты текущего броска к истории бросков участников
        /// </summary>
        private void ApplyRollResults(TurnSnapshot snapshot, DuelContext context)
        {
            foreach (KeyValuePair<string, RollHistoryEntry> pair in snapshot.ParticipantCurrentRolls)
            {
                string participantId = pair.Key;
                RollHistoryEntry rollEntry = pair.Value;

                if (!context.Participants.TryGetValue(participantId, out DuelParticipantState participant))
                {
                    ServiceDebug.LogError($"Участник '{participantId}' не найден в контексте, результат броска пропущен");
                    continue;
                }

                FightParticipantState fightState = participant.FightState;
                fightState.MarkThrow();
                fightState.RegisterRawRollResult(rollEntry.DiceInstanceId, rollEntry.RawResult);
                fightState.UpdateLastRollFinalResult(rollEntry.FinalResult);
            }
        }

        /// <summary>
        /// Отмечает использование ресурсов по данным снапшота
        /// </summary>
        private void ApplyUsage(TurnSnapshot snapshot, DuelContext context)
        {
            foreach (UsageMutation mutation in snapshot.UsageMutations)
            {
                if (!context.Participants.TryGetValue(mutation.OwnerParticipantId, out DuelParticipantState participant))
                {
                    ServiceDebug.LogError($"Участник '{mutation.OwnerParticipantId}' из мутации использования не найден");
                    continue;
                }

                FightParticipantState fightState = participant.FightState;

                switch (mutation.SourceKind)
                {
                    case EffectSourceKind.Dice:
                        fightState.MarkDiceUsed(mutation.SourceInstanceId, mutation.TargetParticipantId);
                        break;

                    case EffectSourceKind.Consumable:
                        fightState.MarkConsumableUsed(mutation.SourceInstanceId, mutation.TargetParticipantId);
                        break;
                }
            }
        }

        /// <summary>
        /// Запись снапшота хода в историю дуэли
        /// </summary>
        private void AppendHistory(TurnSnapshot snapshot, DuelContext context)
        {
            if (!context.History.TryGetLastEntry(out FightHistoryEntry currentFightEntry))
            {
                ServiceDebug.LogError("Текущая запись боя в истории отсутствует, ход не записан");
                return;
            }

            TurnHistoryEntry newTurnEntry = new TurnHistoryEntry(
                snapshot,
                context.Progress
            );

            currentFightEntry.AddEntry(newTurnEntry);
        }
        
        #endregion
    }
}