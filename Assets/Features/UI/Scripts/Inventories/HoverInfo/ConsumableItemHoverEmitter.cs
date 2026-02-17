using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Контроллер запросов вывода информации о ячейке инвентаря расходников
    /// </summary>
    public class ConsumableItemHoverEmitter : GenericInventoryItemHoverInfoEmitter<ConsumablesInventory, ConsumableItemCell, ConsumableData, ConsumableType> { }
}