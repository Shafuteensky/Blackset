using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Контроллер иконки перетаскиваемого из инвентаря расходника
    /// </summary>
    public class ConsumableDragIconController : GenericDragIconController<ConsumablesInventory, ConsumableItemCell, ConsumableData, ConsumableType> { }
}