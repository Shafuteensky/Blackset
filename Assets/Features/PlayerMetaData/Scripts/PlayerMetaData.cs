using System.Collections.Generic;
using Blackset.Inventory.Inventories;
using Blackset.Inventory.ItemPools;

namespace Blackset.Player
{
    /// <summary>
    /// Структура мета-данных игрока
    /// </summary>
    public class PlayerMetaData
    {
        /// <summary>
        /// Количество софт-валюты
        /// </summary>
        public int Money { get; private set; }
        /// <summary>
        /// Суммарный опыт
        /// </summary>
        public int SumExperience { get; private set; }
        /// <summary>
        /// Опыт на текущем уровне
        /// </summary>
        public int ThisLvlExperience { get; private set; }
        /// <summary>
        /// Уровень
        /// </summary>
        public int Level { get; private set; }
        
        public void AddMoney(int amount) => Money += amount;
    }
}
