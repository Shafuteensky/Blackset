using System;
using Blackset.Data.Registries;
using Blackset.Effects;
using Blackset.Inventories.Cells;
using Blackset.Inventories.Items;
using Blackset.Player;
using Blackset.UI.InventoryManagement;
using Extensions.Singleton;

namespace Blackset.Shop
{
    /// <summary>
    /// Контроллер магазина
    /// </summary>
    /// <remarks>
    /// Обрабатывает операции покупки и продажи предметов
    /// </remarks>
    public sealed class ShopController : MonoBehaviourSingleton<ShopController>
    {
        private const int DEFAULT_BUY_ITEM_AMOUNT = 1;
        
        #region События

        /// <summary>
        /// Невозможность покупки из-за нехватки денег у игрока
        /// </summary>
        /// <typeparam name="int">Недостаточное количество денег</typeparam>
        public event Action<int> onNotEnoughMoney;
        /// <summary>
        /// Факт покупки определенного предмета (добавление его в инвентарь игрока)
        /// </summary>
        public event Action<ItemContext, int> onItemBought;
        /// <summary>
        /// Факт покупки определенного предмета (добавление его в инвентарь игрока)
        /// </summary>
        public event Action<ItemContext, int> onSecretItemBought;

        #endregion

        /// <summary>
        /// Попытка купить предмет
        /// </summary>
        public bool TryBuyItem(InventoryItemElement itemElement, bool isHidden = false, bool isSetPack = false)
        {
            // Игровые данные
            PlayerDataFacade playerData = GameData.Instance.PlayerDataFacade;
            
            // Данные покупаемого предмета
            InventoryCell cell = itemElement.DataContainer.GetById(itemElement.EntryId);
            InventoryItem shopItem = itemElement.DataContainer.GetCellItemData(itemElement.EntryId);
            
            // Деньги игрока и стоимость предмета
            int playerMoney = playerData.MetaData.Data.Money;
            int itemPrice = shopItem.GetBuyPrice(itemElement.DataContainer.GetCellTypeData(itemElement.EntryId));
            
            // Если предмет продавался как рандомный пак, то цена от его набора
            if (isSetPack && shopItem is EffectingItem effectingItem) itemPrice = effectingItem.Set.GetBuyPrice();
                
            // Если у игрока недостаточно денег — покупка не совершается
            if (playerMoney < itemPrice)
            {
                onNotEnoughMoney?.Invoke(Math.Abs(playerMoney - itemPrice));
                return false;
            }
            
            // Денег достаточно — отнять деньги, добавить в инвентарь, убрать из магазина
            playerData.MetaData.RemoveMoney(itemPrice);
            playerData.Inventory.AddNewItem(cell.Item, DEFAULT_BUY_ITEM_AMOUNT);
            itemElement.DataContainer.RemoveItem(cell, DEFAULT_BUY_ITEM_AMOUNT);

            if (isHidden) onSecretItemBought?.Invoke(cell.Item, DEFAULT_BUY_ITEM_AMOUNT);
            else onItemBought?.Invoke(cell.Item, DEFAULT_BUY_ITEM_AMOUNT);
            
            return true;
        }
    }
}