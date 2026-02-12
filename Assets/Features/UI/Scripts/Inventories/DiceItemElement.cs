using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Элемент UI фабрики содержимого инвентаря дайсов
    /// </summary>
    public class DiceItemElement : GenericInventoryItemElement<DicesInventory, DiceItemCell, DiceData, DiceType> { }
}