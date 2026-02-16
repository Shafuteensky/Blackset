using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Контроллер запросов вывода информации о ячейке инвентаря дайсов
    /// </summary>
    public class DiceItemHoverEmitter : GenericInventoryItemHoverInfoEmitter<DicesInventory, DiceItemCell, DiceData, DiceType> { }
}