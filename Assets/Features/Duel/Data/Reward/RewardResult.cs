using System.Collections.Generic;
using Blackset.Data;

namespace Blackset.Duel.Requests
{
    /// <summary>
    /// Выданная игроку награда за дуэль
    /// </summary>
    public class RewardResult
    {
        /// <summary>
        /// Количество полученного опыта
        /// </summary>
        public int ExpDelta;
        /// <summary>
        /// Количество полученной валюты
        /// </summary>
        public int CurrencyDelta;
        /// <summary>
        /// Полученные дайсы
        /// </summary>
        public List<DiceItemContext> Dices;
        /// <summary>
        /// Полученные расходники
        /// </summary>
        public List<ConsumableItemContext> Consumables;
    }
}