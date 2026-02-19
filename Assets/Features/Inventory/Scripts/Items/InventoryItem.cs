using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using UnityEngine;

namespace Features.Inventory.Scripts.Items
{
    public class InventoryItem : BaseData
    {
        /// <summary>
        /// Класс предмета
        /// </summary>
        public ItemClass ItemClass => itemClass;
        
        [Header("Принадлежность"), Space]
        [SerializeField]
        protected ItemClass itemClass =  ItemClass.Any;
        
        [Header("Ценники")]
        [SerializeField]
        [Range(1, 100)]
        [Tooltip("Базовая стоимость в софт-валюте (без учета редкости и типа)")]
        private int price = 1;

        /// <summary>
        /// Получить актуальную цену предмета
        /// </summary>
        /// <returns>Стоимость предмета с учетом редкости и типа</returns>
        public int GetPrice(InventoryItemType type) => price * type.PriceMultiplier;
    }
}