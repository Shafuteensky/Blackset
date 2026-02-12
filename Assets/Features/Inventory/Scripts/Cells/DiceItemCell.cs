using System;
using Blackset.Data;
using Blackset.Data.Items.Types;

namespace Blackset.Inventory.Cells
{
    /// <summary>
    /// Предмет ячейки инвентаря, содержащей дайс
    /// </summary>
    [Serializable]
    public sealed class DiceItemCell : BaseItemCell<DiceData, DiceType>
    {
        public DiceItemCell(string itemId, string itemTypeId, int itemAmount = 1, bool isDefault = false, bool isEmpty = false) 
            : base(itemId, itemTypeId, itemAmount, isDefault, isEmpty) { }
    }
}