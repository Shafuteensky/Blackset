using Blackset.Data.Items.Types;
using UnityEngine;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Реестр типов предметов
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(InventoryItemTypesRegistry),
        menuName = "Blackset/Registries/" + nameof(InventoryItemTypesRegistry))]
    public class InventoryItemTypesRegistry : BaseDataRegistry<InventoryItemType> { }
}