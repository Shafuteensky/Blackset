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
    public sealed class DicesInventory : BaseInventory<DiceItemCell, DiceData, DiceType> { }
}