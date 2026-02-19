using Features.Inventory.Scripts.Items;
using UnityEngine;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Реестр данных предметов
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(InventoryItemsRegistry),
        menuName = "Blackset/Registries/" + nameof(InventoryItemsRegistry))]
    public class InventoryItemsRegistry : BaseDataRegistry<InventoryItem> { }
}