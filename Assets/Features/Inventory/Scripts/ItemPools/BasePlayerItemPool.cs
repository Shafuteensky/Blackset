using System.Collections.Generic;
using UnityEngine;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Inventories;
using Blackset.Inventory.Cells;

namespace Blackset.Inventory.ItemPools
{
    /// <summary>
    /// Базовый пул игрока 
    /// </summary>
    public abstract class BasePlayerItemPool<TItem, TData, TType> : BaseInventory<TItem, TData, TType>
        where TItem : BaseItemCell<TData, TType>
        where TData : BaseData
        where TType : BaseItemType
    {
        /// <summary>
        /// Разрешенный тип предметов (оставить пустым, если без ограничений)
        /// </summary>
        public TType AllowedItemType => allowedItemType;
        
        [Header("Настройки содержимого пула"), Space]
        
        [SerializeField]
        [Tooltip("Количество слотов пула")]
        [Range(1, 10)]
        protected int slotsCount = 2;
        
        [SerializeField]
        [Tooltip("Разрешенный тип предметов (оставить пустым, если без ограничений)")]
        protected TType allowedItemType;
        [SerializeField]
        [Tooltip("Дефолтные предметы, не дающие ячейкам пула быть пустующими")]
        protected List<TData> defaultItemsData = new List<TData>();
    }
}
