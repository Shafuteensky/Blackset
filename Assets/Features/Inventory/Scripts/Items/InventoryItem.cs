using System;
using System.Collections.Generic;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Inventories.Scripts.Items;
using Blackset.ItemsRestrictions;
using UnityEngine;

namespace Blackset.Inventories.Items
{
    public class InventoryItem : BaseData
    {
        /// <summary>
        /// Класс предмета
        /// </summary>
        public ItemClass ItemClass => itemClass;
        /// <summary>
        /// Базовая цена предмета (без зависимости от типа, редкости и модификаторов)
        /// </summary>
        public int RawPrice => price;
        
        [Header("Доступ"), Space]
        [SerializeField] private ItemAvailability baseAvailability = ItemAvailability.All;
        [SerializeField] private List<ItemRestriction> restrictions = new();
        
        [Header("Ценники"), Space]
        [Tooltip("Базовая стоимость в софт-валюте (без учета редкости и типа)")]
        [Range(0, 100)]
        [SerializeField] private int price = 1;
        
        [Header("Принадлежность"), Space]
        [SerializeField] protected ItemClass itemClass = ItemClass.Any;

        /// <summary>
        /// Получить актуальную цену предмета для продажи
        /// </summary>
        /// <param name="type">Тип предмета для учета наценки по типу</param>
        /// <returns>Стоимость продажи предмета с учетом редкости, типа и балансных модификаторов</returns>
        // TODO Добавить модификатор от редкости
        public int GetSellPrice(InventoryItemType type) => 
            (int)Math.Round(price * type.PriceMultiplier * GameData.Instance.ShopConfig.ItemsBuySell.ItemSellModifier);
        
        /// <summary>
        /// Получить актуальную цену предмета для покупки
        /// </summary>
        /// <param name="type">Тип предмета для учета наценки по типу</param>
        /// <returns>Стоимость покупки предмета с учетом редкости, типа и балансных модификаторов</returns>
        // TODO Добавить модификатор от редкости
        public int GetBuyPrice(InventoryItemType type = null)
        {
            float typeModifier = 1f;
            if (type != null) typeModifier = type.PriceMultiplier;
            return (int)Math.Round(price * typeModifier * GameData.Instance.ShopConfig.ItemsBuySell.ItemBuyModifier);
        }
        
        #region Доступность предмета
        
        /// <summary>
        /// Уровень доступности предмета
        /// </summary>
        public ItemAvailability GetAvailability()
        {
            ItemAvailability availability = baseAvailability;

            foreach (ItemRestriction restriction in restrictions)
            {
                if (restriction == null) continue;

                ItemAvailability blockedAvailability = restriction.GetBLockedAvailability(this);
                availability &= ~blockedAvailability;

                if (availability == ItemAvailability.None) break;
            }

            return availability;
        }

        /// <summary>
        /// Доступен ли предмет к награде
        /// </summary>
        /// <returns>true если доступен, иначе false</returns>
        public bool IsAvailableForReward() => (GetAvailability() & ItemAvailability.Reward) != 0;

        /// <summary>
        /// Доступен ли предмет к покупке
        /// </summary>
        /// <returns>true если доступен, иначе false</returns>
        public bool IsAvailableForPurchase() => (GetAvailability() & ItemAvailability.Purchase) != 0;
        
        #endregion
    }
}