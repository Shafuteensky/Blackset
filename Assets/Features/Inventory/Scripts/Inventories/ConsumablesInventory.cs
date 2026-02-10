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
    public sealed class ConsumablesInventory : BaseInventory<ConsumableItemCell, ConsumableData, ConsumableType> { }
}