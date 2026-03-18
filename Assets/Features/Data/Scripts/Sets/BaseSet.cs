using System;
using System.Collections.Generic;
using Blackset.Data.Base;
using Blackset.Data.Registries;
using Blackset.Inventories.Items;
using UnityEngine;

namespace Blackset.Data
{
    /// <summary>
    /// Базовый набор игровых единиц (дайсы/расходники)
    /// </summary>
    public abstract class BaseSet : BaseData
    {
        /// <summary>
        /// Цвет набора
        /// </summary>
        public Color Color => color;
        /// <summary>
        /// Предметы в наборе
        /// </summary>
        public List<InventoryItem> ItemsInSet => itemsInSet;
        
        [Header("Набор"), Space]
        [SerializeField] protected Color color;
        
        [Header("Ценник"), Space]
        [Tooltip("Ценник не фиксированный, а усредненный из всех предметов в наборе")]
        [SerializeField] private bool isPriceAveraged;
        [Tooltip("Базовая стоимость рандомного пака набора в софт-валюте")]
        [Range(0, 100)]
        [SerializeField] private int price = 5;
        
        [Header("Предметы в наборе"), Space]
        [SerializeField] protected List<InventoryItem> itemsInSet;
        
        /// <summary>
        /// Стоимость покупки пака набора
        /// </summary>
        public int GetBuyPrice()
        {
            float buyPrice = 0;
            
            if (isPriceAveraged)
            {
                foreach (InventoryItem item in itemsInSet)
                {
                    // Без модификатора покупки, чтобы не было невыгодного для игрока смещения стоимости полученного предмета
                    buyPrice += item.RawPrice;
                }
                buyPrice /= itemsInSet.Count; 
            }
            else 
                buyPrice = price * GameData.Instance.ShopConfig.ItemsBuySell.ItemBuyModifier;

            return (int)Math.Round(buyPrice);
        }
    }
}