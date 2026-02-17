using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Контроллер выводимой информации о поредмете инвентаря расходников
    /// </summary>
    public class ConsumableItemInfoPopupController : GenericInventoryItemInfoPopupController<ConsumablesInventory, ConsumableItemCell, ConsumableData, ConsumableType> { }
}