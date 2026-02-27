using Blackset.Duel.Context;

namespace Blackset.Duel.Snapshots
{
    /// <summary>
    /// Перенос финально рассчитанных данных хода из снапшота в "истину" дуэли
    /// </summary>
    // TODO Вызывается один раз в конце RollResolveState (после расчётов и презентации).
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
        /// <param name="snapshot">Данные зарезолвенного снапшота</param>
        public void Commit(TurnSnapshot snapshot)
        {
            // 1) Применение результатов к счетам/статусам участников
            ApplyScores(snapshot);

            // 2) Отметка использования ресурсов (кубы/расходники/пасы)
            ApplyUsage(snapshot);

            // 3) Обновление knowledge (раскрытие кубов/расходников при первом использовании)
            ApplyKnowledge(snapshot);

            // 4) Запись истории хода (для UI/логов/повторов)
            AppendHistory(snapshot);

            // 5) Прочие фиксации (если у тебя есть: длительные эффекты, таймеры, стаки и т.д.)
            ApplyOngoing(snapshot);
        }

        #region Применение расчитанных изменений

        private void ApplyScores(TurnSnapshot snapshot)
        {
            // TODO: перенеси итоговые изменения счета из snapshot в context
            // Пример смысла:
            // context.Participants[playerId].Score = snapshot.FinalScores[playerId];
            // context.Participants[enemyId].Score = snapshot.FinalScores[enemyId];
        }

        private void ApplyUsage(TurnSnapshot snapshot)
        {
            // TODO: отметь что конкретно было использовано в этом ходу
            // - какой дайс реально выбран/сыгран (и где в сетах он лежит)
            // - какой расходник сыгран (и сожжен/помечен использованным)
            // - был ли пас
        }

        private void ApplyKnowledge(TurnSnapshot snapshot)
        {
            // TODO: если в snapshot есть факты "что раскрыто впервые" — перенеси в context.Knowledge
            // - reveal dice id
            // - reveal consumable id
        }

        private void AppendHistory(TurnSnapshot snapshot)
        {
            // TODO: добавь запись в историю (context.History.Add(...))
            // В записи обычно: intents, declared номинал, real dice id, roll value, applied effects, score delta
        }

        private void ApplyOngoing(TurnSnapshot snapshot)
        {
            // TODO: если есть длительные эффекты/флаги/модификаторы на несколько ходов — зафиксируй их тут
        }
        
        #endregion
    }
}