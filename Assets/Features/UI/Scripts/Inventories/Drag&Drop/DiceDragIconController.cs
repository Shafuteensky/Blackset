using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Контроллер иконки перетаскиваемого из инвентаря дайса
    /// </summary>
    public class DiceDragIconController : GenericDragIconController<DicesInventory, DiceItemCell, DiceData, DiceType> { }
}