using Blackset.Duel.Context;
using Blackset.Duel.Snapshots;

namespace Blackset.Duel.History
{
    /// <summary>
    /// Запись истории дуэли за ход (исторические данные хода)
    /// </summary>
    public class TurnHistoryEntry
    {
        /// <summary>
        /// Снапшот хода дуэли
        /// </summary>
        public TurnSnapshot Snapshot { get; }
        /// <summary>
        /// Прогресс дуэли
        /// </summary>
        public DuelProgressContext DuelProgress { get; }

        /// <summary>
        /// Создание новой исторической записи хода
        /// </summary>
        /// <param name="snapshot">Снапшот данных хода</param>
        /// <param name="duelProgress">Прогресс дуэли на текущий ход</param>
        /// <param name="participantStates">Состояния участников в текущем ходу</param>
        public TurnHistoryEntry(TurnSnapshot snapshot, DuelProgressContext duelProgress)
        {
            Snapshot = snapshot.CloneDeep();
            DuelProgress = duelProgress;
        }
    }
}