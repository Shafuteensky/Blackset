using System;
using Blackset.Data;
using Blackset.Data.Items.Types;

namespace Blackset.Inventory.Cells
{
    /// <summary>
    /// Предмет ячейки инвентаря, содержащей расходник
    /// </summary>
    [Serializable]
    public sealed class ConsumableItemCell : BaseItemCell<ConsumableData, ConsumableType>
    {
        public ConsumableItemCell(string itemId, string itemTypeId, int itemAmount = 1) : base(itemId, itemTypeId, itemAmount) { }
    }
}