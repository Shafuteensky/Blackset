using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// UI фабрика содержимого инвентаря дайсов
    /// </summary>
    public class DiceItemsFactory : GenericInventoryItemFactory<DicesInventory, DiceItemCell, DiceData, DiceType> { }
}