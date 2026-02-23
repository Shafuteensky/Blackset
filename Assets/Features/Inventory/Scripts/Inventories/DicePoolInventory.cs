using System;
using Blackset.Data;
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
        
        /// <summary>
        /// Проверка на достаточность дюджета
        /// </summary>
        /// <param name="thisItem">Предмет в этой ячейке</param>
        /// <param name="thatItem">Предмет</param>
        /// <returns>true если бюджета хватает, иначе false</returns>
        public bool IsAllowedByBudget(InventoryItem thisItem, InventoryItem thatItem)
        {
            if (thisItem is not DiceItem thisDice) return true;

            if (playerData == null)
            {
                ServiceDebug.LogError("Ссылка на данные игрока не назначена, проверка бюджета не выполнена");
                return false;
            }
            
            int freeBudget = playerData.GetFreeDiceBudget();

            int refund = 0;
            if (thatItem is DiceItem thatDice) refund = thatDice.BudgetPrice;

            int available = freeBudget + refund;
            
            int required = thisDice.BudgetPrice;
            
            Debug.Log("free " + freeBudget + ", available "  + available + ", required " + required);
            
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