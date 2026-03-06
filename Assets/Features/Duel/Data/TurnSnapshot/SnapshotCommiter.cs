using System.Collections.Generic;
using Blackset.Duel.Context;
using Blackset.Duel.History;
using Blackset.Duel.Participants;
using Blackset.Duel.TurnIntents;
using Extensions.Helpers;
using Extensions.Log;

namespace Blackset.Duel.Snapshots
{
    /// <summary>
    /// Перенос финально рассчитанных данных хода из снапшота в "истину" дуэли
    /// </summary>
    public class SnapshotCommiter 
    {
        private readonly DuelContext context;

        /// <summary>
        /// Новый коммитер расчитанной информации дуэли
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        public SnapshotCommiter(DuelContext context)
        {
            this.context = context;
        }
        
        /// <summary>
        /// Коммит снапшота
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        /// <param name="resolvedSnapshot">Данные зарезолвенного снапшота</param>
        public void Commit(TurnSnapshot resolvedSnapshot)
        {
            if (context.Participants.Count == 0)
            {
                ServiceDebug.LogError($"Участники не зарегестрированны в {nameof(TurnSnapshot)}, снапшот не применен");
                return;
            }
            
            // 1) Применение результатов к счетам/статусам участников
            ApplyScores(resolvedSnapshot);
            // 2) Отметка использования ресурсов (кубы/расходники/пасы)
            ApplyUsage(resolvedSnapshot);
            // 3) Обновление knowledge (раскрытие кубов/расходников при первом использовании)
            ApplyKnowledge(resolvedSnapshot);
            // 4) Запись истории хода (для UI/логов/повторов)
            AppendHistory(resolvedSnapshot);
            // 5) Прочие фиксации (если у тебя есть: длительные эффекты, таймеры, стаки и т.д.)
            ApplyOngoing(resolvedSnapshot);
        }

        #region Применение расчитанных изменений

        /// <summary>
        /// Применяет финальные счета участников из снапшота
        /// </summary>
        private void ApplyScores(TurnSnapshot snapshot)
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
        private void ApplyUsage(TurnSnapshot snapshot)
        {
            foreach (KeyValuePair<string, TurnIntent> pair in snapshot.ParticipantIntents)
            {
                string participantId = pair.Key;
                TurnIntent intent = pair.Value;

                if (!context.Participants.TryGetValue(participantId, out DuelParticipantState participant))
                {
                    ServiceDebug.LogError($"Участник '{participantId}' из снапшота не найден в контексте, пропущен");
                    continue;
                }

                // Участник спасовал — ресурсы не тратились
                if (intent.IsPass)
                    continue;

                FightParticipantState fightState = participant.FightState;

                fightState.MarkThrow();

                if (!string.IsNullOrEmpty(intent.ChosenDice))
                    fightState.MarkDiceUsed(intent.ChosenDice);

                if (intent.ConsumableChosen && !string.IsNullOrEmpty(intent.ChosenConsumable))
                    fightState.MarkConsumableUsed(intent.ChosenConsumable);
            }
        }

        /// <summary>
        /// Раскрывает дайсы участников, использованные в этот ход
        /// </summary>
        private void ApplyKnowledge(TurnSnapshot snapshot)
        {
            foreach (KeyValuePair<string, KnowledgeState> pair in snapshot.PlayerKnowledge)
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
        private void AppendHistory(TurnSnapshot snapshot)
        {
            if (!context.History.TryGetLastEntry(out FightHistoryEntry currentFightEntry))
            {
                currentFightEntry = new FightHistoryEntry();
                context.History.AddEntry(currentFightEntry);
            }

            Dictionary<string, DuelParticipantState> participantStatesCopy = CollectionCopy.DictionaryShallow(context.Participants);

            TurnHistoryEntry newTurnEntry = new TurnHistoryEntry(
                snapshot,
                context.Progress,
                participantStatesCopy
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