using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;
using Unity.VisualScripting;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// UI фабрика содержимого инвентаря расходников
    /// </summary>
    public class ConsumablesItemsFactory : GenericInventoryItemFactory<ConsumablesInventory, ConsumableItemCell, ConsumableData, ConsumableType>
    {
        protected override void PrepareDropZone()
        {
            ConsumableItemElement dropZone = transform.AddComponent<ConsumableItemElement>();
            dropZone.InitializeElement(inventory);
        }
    }
}