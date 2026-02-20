using System;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.Effects;
using Features.Inventory.Scripts.Items;
using UnityEngine;

namespace Blackset.Inventories
{
    /// <summary>
    /// Данные дефолтной ячейки инвентаря BaseInventory
    /// </summary>
    /// <typeparam name="TData">Тип данных предмета</typeparam>
    /// <typeparam name="TType">Тип типа предмета</typeparam>
    [Serializable]
    public struct DefaultInventoryItemCell
    {
        [SerializeField]
        public InventoryItem ItemData;
        [SerializeField]
        public InventoryItemType ItemTypeData;
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