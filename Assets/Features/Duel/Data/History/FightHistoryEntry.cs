using System.Collections.Generic;
using System.Linq;
using Extensions.Helpers;

namespace Blackset.Duel.History
{
    /// <summary>
    /// История боя
    /// </summary>
    public sealed class FightHistoryEntry : IHistoryEvents<TurnHistoryEntry>
    {
        /// <summary>
        /// Записи истории дуэли по ходам
        /// </summary>
        public List<TurnHistoryEntry> TurnsHistory => turnsHistory;
        
        private readonly List<TurnHistoryEntry> turnsHistory = new();
        
        /// <summary>
        /// Внести запись в историю
        /// </summary>
        /// <param name="entry">Исторические данные хода</param>
        public void AddEntry(TurnHistoryEntry entry)
        {
            turnsHistory.Add(entry);
        }

        /// <summary>
        /// Получить последнюю историческую запись
        /// </summary>
        /// <returns>Исторические данные боя</returns>
        public bool TryGetLastEntry(out TurnHistoryEntry entry)
        {
            entry = turnsHistory.Count > 0 ? turnsHistory.Last() : default;
            return turnsHistory.Count > 0;
        }
    }
}