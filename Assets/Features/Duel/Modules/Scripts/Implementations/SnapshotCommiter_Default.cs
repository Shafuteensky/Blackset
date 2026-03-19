using System.Collections.Generic;
using Blackset.Duel.Context;
using Blackset.Duel.History;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Extensions.Helpers;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Duel.Snapshots
{
    /// <summary>
    /// Перенос финально рассчитанных данных хода из снапшота в "истину" дуэли
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(SnapshotCommiter_Default),
        menuName = "Blackset/Duel/Modules/" + nameof(SnapshotCommiter_Default))]
    public class SnapshotCommiter_Default : BaseDuelModule, ISnapshotCommiter
    {
        /// <summary>
        /// Коммит снапшота
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        /// <param name="resolvedSnapshot">Данные зарезолвенного снапшота</param>
        public void Commit(TurnSnapshot resolvedSnapshot, DuelContext context)
        {
            if (context.Participants.Count == 0)
            {
                ServiceDebug.LogError($"Участники не зарегестрированны в {nameof(TurnSnapshot)}, снапшот не применен");
                return;
            }
            
            // 1) Применение результатов к счетам/статусам участников
            ApplyScores(resolvedSnapshot, context);
            // 2) Отметка использования ресурсов (кубы/расходники/пасы)
            ApplyUsage(resolvedSnapshot, context);
            // 3) Обновление knowledge (раскрытие кубов/расходников при первом использовании)
            ApplyKnowledge(resolvedSnapshot, context);
            // 4) Запись истории хода (для UI/логов/повторов)
            AppendHistory(resolvedSnapshot, context);
            // 5) Прочие фиксации (если у тебя есть: длительные эффекты, таймеры, стаки и т.д.)
            ApplyOngoing(resolvedSnapshot);
        }

        #region Применение расчитанных изменений

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
                
                pair.Value.FightState.UpdateScore(snapshotScore);
            }
        }

        /// <summary>
        /// Отмечает использование ресурсов (дайсы, расходники, броски, пасы)
        /// на основе намерений участников из снапшота
        /// </summary>
        // TODO Должен ли отмечать на основе намерений? Или на основе обработанных в FSM фактов (добавить доп. поля в резолвер)?
        private void ApplyUsage(TurnSnapshot snapshot, DuelContext context)
        {
            foreach (var pair in snapshot.ParticipantStates)
            {
                string participantId = pair.Key;
                TurnParticipantState participantState = pair.Value;

                if (!context.Participants.TryGetValue(participantId, out DuelParticipantState participant))
                {
                    ServiceDebug.LogError($"Участник '{participantId}' из снапшота не найден в контексте, пропущен");
                    continue;
                }

                // Участник спасовал — ресурсы не тратились
                if (participantState.HasPassed.Value)
                    continue;

                FightParticipantState fightState = participant.FightState;

                fightState.MarkThrow();

                if (participantState.IsDiceChosen.Value)
                    fightState.MarkDiceUsed(participantState.SelectedDice.Value);

                if (participantState.IsConsumableChosen.Value)
                    fightState.MarkConsumableUsed(participantState.SelectedConsumable.Value);
            }
        }

        /// <summary>
        /// Раскрывает дайсы участников, использованные в этот ход
        /// </summary>
        private void ApplyKnowledge(TurnSnapshot snapshot, DuelContext context)
        {
            foreach (KeyValuePair<string, KnowledgeState> pair in snapshot.ParticipantKnowledge)
            {
                if (!context.Knowledge.TryGetValue(pair.Key, out KnowledgeState knowledge))
                {
                    ServiceDebug.LogError($"KnowledgeState для '{pair.Key}' не найден, пропущен");
                    continue;
                }

                foreach (string diceId in pair.Value.RevealedDices)
                    knowledge.RevealDice(diceId);
            }
        }

        /// <summary>
        /// Запись снапшота хода в историю дуэли
        /// Добавляет <see cref="TurnHistoryEntry"/> в текущий <see cref="FightHistoryEntry"/>.
        /// Если запись текущего боя отсутствует — создаёт новую.
        /// </summary>
        private void AppendHistory(TurnSnapshot snapshot, DuelContext context)
        {
            if (!context.History.TryGetLastEntry(out FightHistoryEntry currentFightEntry))
            {
                currentFightEntry = new FightHistoryEntry();
                context.History.AddEntry(currentFightEntry);
            }

            Dictionary<string, DuelParticipantState> participantStatesCopy = CollectionCopy.DictionaryShallow(context.Participants);

            TurnHistoryEntry newTurnEntry = new TurnHistoryEntry(
                snapshot,
                context.Progress
            );

            currentFightEntry.AddEntry(newTurnEntry);
        }

        /// <summary>
        /// Фиксирует длительные эффекты, модификаторы, флаги на несколько ходов
        /// </summary>
        private void ApplyOngoing(TurnSnapshot snapshot)
        {
            // TODO: если есть длительные эффекты/флаги/модификаторы на несколько ходов
        }
        
        #endregion
    }
}