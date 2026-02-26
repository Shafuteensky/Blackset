using System.Collections.Generic;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;

namespace Blackset.Duel.History
{
    /// <summary>
    /// Запись истории дуэли за ход (исторические данные хода)
    /// </summary>
    public struct DuelTurnHistoryEntry
    {
        /// <summary>
        /// Снапшот хода дуэли
        /// </summary>
        public TurnSnapshot.TurnSnapshot Snapshot;
        /// <summary>
        /// Прогресс дуэли
        /// </summary>
        public DuelProgressContext DuelProgress;
        /// <summary>
        /// Состояния участников <идентификатор, состояние>
        /// </summary>
        public Dictionary<int, DuelParticipantState> participantStates;
    }
}