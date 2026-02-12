using UnityEngine;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;

namespace Blackset.Inventory.Inventories
{
    /// <summary>
    /// Инвентарь дайсов игрока
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Inventory/" + nameof(DicesInventory),
        fileName = nameof(DicesInventory))]
    public sealed class DicesInventory : BaseInventory<DiceItemCell, DiceData, DiceType>
    {
        protected override DiceItemCell CreateCell(string itemId, string itemTypeId, int amount, bool isDefault = false)
        {
            DiceItemCell newCell = new DiceItemCell(itemId, itemTypeId, amount, isDefault);
            return newCell;
        }
        
        protected override DiceItemCell CreateEmptyCell()
        {
            DiceItemCell newCell = new DiceItemCell();
            return newCell;
        }
    }
}