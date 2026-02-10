using System;
using Blackset.Data;
using Blackset.Data.Items.Types;

namespace Blackset.Inventory
{
    /// <summary>
    /// Предмет ячейки инвентаря, содержащей дайс
    /// </summary>
    [Serializable]
    public sealed class DiceItemCellCell : BaseItemCell<DiceData, DiceType>
    {
        public DiceItemCellCell(string itemId, string itemTypeId, int itemAmount = 1) : base(itemId, itemTypeId, itemAmount) { }
    }
}