using System;
using Blackset.Data;
using Blackset.Inventories.Cells;
using Blackset.Inventories.Items;
using Blackset.Player;
using Blackset.UI.InventoryManagement;
using Extensions.Generics;
using Extensions.Log;
using Extensions.ScriptableValues;
using UnityEngine;

namespace Blackset.Shop
{
    /// <summary>
    /// Абстракция кнопки покупки предмета магазина
    /// </summary>
    public class BuyShopItemButton : AbstractHoldButton
    {
        #region События

        /// <summary>
        /// Невозможность покупки из-за нехватки денег у игрока
        /// </summary>
        /// <typeparam name="int">Недостаточное количество денег</typeparam>
        public static event Action<int> onNotEnoughMoney;
        /// <summary>
        /// Факт покупки определенного предмета (боавление его в инвентарь игрока)
        /// </summary>
        public static event Action onItemBought;

        #endregion
        
        [Header("Ячейка"), Space]
        [SerializeField]
        [Tooltip("Опциональная фиксированная стоимость лота (оставить пустым чтобы брать цену от предмета)")]
        protected IntValue fixedPrice;
        
        [Header("Ячейка"), Space]
        [SerializeField]
        protected InventoryItemElement itemElement;
        
        [Header("Игровые данные"), Space]
        [SerializeField]
        protected PlayerDataFacade playerData;
        
        public override void OnButtonClick()
        {
            if (playerData == null || itemElement == null)
            {
                ServiceDebug.LogError("Не все ссылки заполнены");
                return;
            }

            TryBuyItem();
        }

        protected void TryBuyItem()
        {
            InventoryCell cell = itemElement.Inventory.GetById(itemElement.ItemCellId);
            InventoryItem shopItem = itemElement.Inventory.GetCellItemData(itemElement.ItemCellId);
            int playerMoney = playerData.MetaData.Data.Money;
            
            int itemPrice;
            if (fixedPrice != null) itemPrice = fixedPrice.Value;
            else itemPrice = shopItem.GetPrice(itemElement.Inventory.GetCellTypeData(itemElement.ItemCellId));
            
            if (playerMoney < itemPrice)
            {
                onNotEnoughMoney?.Invoke(Math.Abs(playerMoney - itemPrice));
                return;
            }
            
            playerData.MetaData.RemoveMoney(itemPrice);
            if (shopItem is DiceItem) playerData.DicesInventory.AddItem(cell);
            else if (shopItem is ConsumableItem) playerData.ConsumablesInventory.AddItem(cell);
            itemElement.Inventory.RemoveItem(cell);

            onItemBought?.Invoke();
        }
    }
}