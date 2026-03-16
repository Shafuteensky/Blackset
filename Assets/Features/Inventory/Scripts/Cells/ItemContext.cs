using System;
using Features.Inventory.Scripts.Items;

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
    }
}