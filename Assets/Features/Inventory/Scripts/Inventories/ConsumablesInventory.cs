using UnityEngine;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;

namespace Blackset.Inventory.Inventories
{
    /// <summary>
    /// Инвентарь расходников игрока
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Inventory/" + nameof(ConsumablesInventory),
        fileName = nameof(ConsumablesInventory))]
    public sealed class ConsumablesInventory : BaseInventory<ConsumableItemCell, ConsumableData, ConsumableType>
    {
        protected override ConsumableItemCell CreateCell(string itemId, string itemTypeId, int amount, bool isDefault = false)
        {
            ConsumableItemCell newCell = new ConsumableItemCell(itemId, itemTypeId, amount, isDefault);
            return newCell;
        }
        
        protected override ConsumableItemCell CreateEmptyCell()
        {
            ConsumableItemCell newCell = new ConsumableItemCell("", "", 0, true, true);
            return newCell;
        }
    }
}