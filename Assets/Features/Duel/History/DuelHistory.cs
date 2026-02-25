using System.Collections.Generic;
using System.Linq;

namespace Blackset.Duel.History
{
    /// <summary>
    /// История дуэли
    /// </summary>
    public class DuelHistory
    {
        /// <summary>
        /// Записи истории дуэли по ходам
        /// </summary>
        public List<DuelTurnHistoryEntry> DuelTurnsHistory;
        
        /// <summary>
        /// Внести запись в историю
        /// </summary>
        /// <param name="entry">Исторические данные хода</param>
        public void AddTurnEntry(DuelTurnHistoryEntry entry)
        {
            DuelTurnsHistory.Add(entry);
        }

        /// <summary>
        /// Получить последнюю историческую запись
        /// </summary>
        /// <returns>Исторические данные хода</returns>
        public DuelTurnHistoryEntry GetLastTurnEntry()
        {
            return DuelTurnsHistory.Last();
        }
    }
}