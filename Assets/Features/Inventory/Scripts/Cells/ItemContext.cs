using System;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Inventories.Items;
using Blackset.Inventories.Scripts.Items;
using Extensions.Log;

namespace Blackset.Inventories.Cells
{
    /// <summary>
    /// Контекст данных о предмете
    /// </summary>
    [Serializable]
    public struct ItemContext
    {
        /// <summary>
        /// Идентификатор данных предмета
        /// </summary>
        public string ItemId { get; private set; }
        /// <summary>
        /// Идентификатор данных типа предмета
        /// </summary>
        public string ItemTypeId { get; private set; }

        /// <summary>
        /// Класс предмета
        /// </summary>
        public ItemClass ItemClass { get; private set; }
        
        /// <summary>
        /// Новый контекст данных о предмете
        /// </summary>
        /// <param name="itemId">Идентификатор данных предмета</param>
        /// <param name="itemTypeId">Идентификатор данных типа предмета</param>
        /// <param name="itemClass">Класс предмета</param>
        public ItemContext(string itemId, string itemTypeId, ItemClass itemClass)
        {
            ItemId = itemId;
            ItemTypeId = itemTypeId;
            ItemClass = itemClass;
        }

        #region Данные предмета

        /// <summary>
        /// Получить данные предмета
        /// </summary>
        public InventoryItem GetItemData()
        {
            InventoryItem data;
            GameData gameData = GameData.Instance;
            switch (ItemClass)
            {
                case ItemClass.Consumable:
                {
                    data = gameData.GetConsumable(ItemId);
                    break;
                }
                case ItemClass.Dice:
                {
                    data = gameData.GetDice(ItemId);
                    break;
                }
                default:
                {
                    ServiceDebug.LogError("Необработанный класс предмета");
                    return null;
                }
            }
            
            return data;
        }

        /// <summary>
        /// Получить данные типа предмета
        /// </summary>
        public InventoryItemType GetTypeData()
        {
            InventoryItemType data;
            GameData gameData = GameData.Instance;
            switch (ItemClass)
            {
                case ItemClass.Consumable:
                {
                    data = gameData.GetConsumableType(ItemTypeId);
                    break;
                }
                case ItemClass.Dice:
                {
                    data = gameData.GetDiceType(ItemTypeId);
                    break;
                }
                default:
                {
                    ServiceDebug.LogError("Необработанный класс предмета");
                    return null;
                }
            }

            return data;
        }
        
        #endregion
    }
}