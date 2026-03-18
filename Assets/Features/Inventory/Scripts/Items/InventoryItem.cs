using System.Collections.Generic;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
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
        /// Получить актуальную цену предмета
        /// </summary>
        /// <returns>Стоимость предмета с учетом редкости и типа</returns>
        public int GetPrice(InventoryItemType type) => price * type.PriceMultiplier;
        
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