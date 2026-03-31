using System.Collections.Generic;
using Blackset.Data;
using Blackset.Inventories.Cells;

namespace Blackset.Duel.Requests
{
    /// <summary>
    /// Выдаваемая игроку награда за дуэль
    /// </summary>
    public class DuelRewards
    {
        /// <summary>
        /// Количество полученного опыта
        /// </summary>
        public int ExpDelta { get; }
        /// <summary>
        /// Количество полученной валюты
        /// </summary>
        public int CurrencyDelta { get; }
        /// <summary>
        /// Полученные предметы
        /// </summary>
        public List<ItemContext> Items { get; }

        /// <summary>
        /// Выдаваемая игроку награда за дуэль
        /// </summary>
        /// <param name="expDelta">Количество полученного опыта</param>
        /// <param name="currencyDelta">Количество полученной валюты</param>
        /// <param name="dices">Полученные дайсы</param>
        /// <param name="consumables">Полученные расходники</param>
        public DuelRewards(int expDelta, int currencyDelta, 
            List<ItemContext> items)
        {
            ExpDelta = expDelta;
            CurrencyDelta = currencyDelta;
            Items = items;
        }
    }
}