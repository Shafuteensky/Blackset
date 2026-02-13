using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Координатор drag & drop инвентаря дайсов
    /// </summary>
    public class DiceInventoryDragDropCoordinator : GenericInventoryDragDropCoordinator<DicesInventory, DiceItemCell, DiceData, DiceType> { }
}