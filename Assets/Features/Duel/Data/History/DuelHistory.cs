using System.Collections.Generic;
using System.Linq;

namespace Blackset.Duel.History
{
    /// <summary>
    /// История дуэли
    /// </summary>
    public sealed class DuelHistory : IHistoryEvents<FightHistoryEntry>
    {
        /// <summary>
        /// Записи истории дуэли по боям
        /// </summary>
        public List<FightHistoryEntry> FightsHistory => fightsHistory;
        
        private readonly List<FightHistoryEntry> fightsHistory = new();
        
        /// <summary>
        /// Внести запись в историю
        /// </summary>
        /// <param name="entry">Исторические данные боя</param>
        public void AddEntry(FightHistoryEntry entry)
        {
            fightsHistory.Add(entry);
        }

        /// <summary>
        /// Получить последнюю историческую запись
        /// </summary>
        /// <returns>Исторические данные хода</returns>
        public bool TryGetLastEntry(out FightHistoryEntry entry)
        {
            entry = fightsHistory.Count > 0 ? fightsHistory.Last() : null;
            return fightsHistory.Count > 0;
        }

        /// <summary>
        /// Получить последнюю историческую запись
        /// </summary>
        /// <returns>Исторические данные хода</returns>
        public bool TryGetFightEntryByIndex(int index, out FightHistoryEntry entry)
        {
            if (index < 0 || index >= fightsHistory.Count)
            {
                entry = null;
                return false;
            }
            entry = fightsHistory[index];
            return true;
        }
    }
}