using System.Collections.Generic;
using Blackset.Data;

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
        /// Полученные дайсы
        /// </summary>
        public List<DiceItemContext> Dices { get; }
        /// <summary>
        /// Полученные расходники
        /// </summary>
        public List<ConsumableItemContext> Consumables { get; }

        /// <summary>
        /// Выдаваемая игроку награда за дуэль
        /// </summary>
        /// <param name="expDelta">Количество полученного опыта</param>
        /// <param name="currencyDelta">Количество полученной валюты</param>
        /// <param name="dices">Полученные дайсы</param>
        /// <param name="consumables">Полученные расходники</param>
        public DuelRewards(int expDelta, int currencyDelta, 
            List<DiceItemContext> dices, List<ConsumableItemContext> consumables)
        {
            ExpDelta = expDelta;
            CurrencyDelta = currencyDelta;
            Dices = dices;
            Consumables = consumables;
        }
    }
}