using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Контроллер выводимой инфомрации о поредмете инвентаря дайсов
    /// </summary>
    public class DiceItemInfoPopupController : GenericInventoryItemInfoPopupController<DicesInventory, DiceItemCell, DiceData, DiceType> { }
}