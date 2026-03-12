using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Blackset.UI.HoverInfo;
using UnityEngine;

namespace Blackset.UI.InventoryManagement
{
    /// <summary>
    /// Контроллер запросов вывода информации о ячейке инвентаря
    /// </summary>
    [RequireComponent(typeof(InventoryItemElement))]
    public sealed class InventoryCellHoverInfoEmitter : BaseHoverInfoEmitter<Inventory, InventoryCell, InventoryItemElement> { }
}
