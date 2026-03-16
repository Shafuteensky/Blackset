using System;
using Blackset.Data;
using Blackset.Inventories.Cells;
using Blackset.Inventories.Items;
using Blackset.Player;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Inventories
{
    /// <summary>
    /// Инвентарь с проверкой на бюджет пула дайсов
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(DicePoolInventory),
        menuName = "Blackset/Inventories/" + nameof(DicePoolInventory))]
    public class DicePoolInventory : Inventory
    {
        /// <summary>
        /// Событие нехватки бюджета дайсов
        /// </summary>
        /// <param name="int">Сколько бюджета не хватило</param>
        public static event Action<int> onBudgetCheckFailed;
        
        [Header("Данные игрока (для проверки бюджета пула дайсов)"), Space]
        [SerializeField]
        protected PlayerDataFacade playerData;
        
        protected override bool IsAllowedToReceive(InventoryCell incomingCell, Inventory fromInventory)
        {
            if (incomingCell == null || fromInventory == null) return false;

            InventoryItem incomingItem = incomingCell.GetItemData();
            bool isAllowedByBudget = IsAllowedByBudget(incomingItem);

            return isAllowedByBudget;
        }
        
        /// <summary>
        /// Проверка на достаточность дюджета
        /// </summary>
        /// <returns>true если бюджета хватает, иначе false</returns>
        protected bool IsAllowedByBudget(InventoryItem thisItem)
        {
            if (thisItem is not DiceData thisDice) return true;

            if (playerData == null)
            {
                ServiceDebug.LogError("Ссылка на данные игрока не назначена, проверка бюджета не выполнена");
                return false;
            }
            
            int freeBudget = playerData.GetFreeDiceBudget();
            int available = freeBudget + playerData.MetaData.Data.GetUsedDiceBudgetByPool(this);
            int required = thisDice.BudgetPrice;
            
            if (available < required)
            {
                int notEnough = required - available;
                onBudgetCheckFailed?.Invoke(notEnough);
                return false;
            }
            return true;
        }
    }
}