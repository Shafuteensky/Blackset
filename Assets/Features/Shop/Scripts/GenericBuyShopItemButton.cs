using System;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Effects;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;
using Blackset.Player;
using Blackset.UI.Inventory;
using Extensions.Generics;
using Extensions.Log;
using UnityEngine;

namespace Features.Shop
{
    /// <summary>
    /// Абстракция кнопки покупки предмета магазина
    /// </summary>
    public abstract class GenericBuyShopItemButton<TInventory, TItemCell, TData, TItemType> : GenericHoldButton
        where TInventory : BaseInventory<TItemCell, TData, TItemType>
        where TItemCell : BaseItemCell<TData, TItemType>
        where TData : EffectingItemData
        where TItemType : BaseItemType
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
        protected GenericInventoryItemElement<TInventory, TItemCell, TData, TItemType> itemElement;
        
        [Header("Инвентари"), Space]
        [SerializeField]
        protected BaseInventory<TItemCell, TData, TItemType> playerInventory;
        
        [Header("Игровые данные"), Space]
        [SerializeField]
        protected PlayerDataFacade playerData;
        
        public override void OnButtonClick()
        {
            if (playerData == null || itemElement == null || playerInventory == null)
            {
                ServiceDebug.LogError("Не все ссылки заполнены");
                return;
            }

            TryBuyItem();
        }

        protected void TryBuyItem()
        {
            TItemCell itemCell = itemElement.Inventory.GetById(itemElement.ItemCellId);
            TData shopItem = itemElement.Inventory.GetCellItemData(itemElement.ItemCellId);
            TItemType shopItemType = itemElement.Inventory.GetCellTypeData(itemElement.ItemCellId);
            int playerMoney = playerData.MetaData.Data.Money;
            int itemPrice = shopItem.GetPrice(itemElement.Inventory.GetCellTypeData(itemElement.ItemCellId));
            
            if (playerMoney < itemPrice)
            {
                onNotEnoughMoney?.Invoke(Math.Abs(playerMoney - itemPrice));
                return;
            }
            
            playerData.MetaData.RemoveMoney(itemPrice);
            playerInventory.AddItem(itemCell);
            itemElement.Inventory.RemoveItem(itemCell);

            onItemBought?.Invoke();
        }
    }
}