using System;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using UnityEngine;

namespace Blackset.Inventory.Inventories
{
    /// <summary>
    /// Данные дефолтной ячейки инвентаря BaseInventory
    /// </summary>
    /// <typeparam name="TData">Тип данных предмета</typeparam>
    /// <typeparam name="TType">Тип типа предмета</typeparam>
    [Serializable]
    public struct DefaultInventoryItemCell<TData, TType>
        where TData : BaseData
        where TType : BaseItemType
    {
        [SerializeField]
        public TData ItemData;
        [SerializeField]
        public TType ItemTypeData;
        [SerializeField]
        [Min(1)]
        public int ItemAmount;

        /// <summary>
        /// Заполнены ли данные в структуре
        /// </summary>
        /// <returns></returns>
        public bool IsNull() => ItemData == null && ItemTypeData == null;
    }
}