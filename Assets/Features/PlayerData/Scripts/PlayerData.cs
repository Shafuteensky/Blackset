using System;
using UnityEngine;

namespace Blackset.Player
{
    /// <summary>
    /// Структура данных игрока
    /// </summary>
    public class PlayerData
    {
        #region Meta
        
        /// <summary>
        /// 
        /// </summary>
        public int Money { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int Experience { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int Level { get; private set; }
        
        #endregion
        
        #region Inventories
        
        // public DicesInventory Dices { get; private set; }
        // public Dictionary<string, DicesPoolRow> DicesPool { get; private set; }
        // public ConsumablesInventory Consumables { get; private set; }
        // public Dictionary<string, ConsumablesPoolRow> ConsumablesPool { get; private set; }
        // public ItemsInventory Items { get; private set; }
        
        #endregion
        
        public void AddMoney(int amount) => Money += amount;
    }
}
