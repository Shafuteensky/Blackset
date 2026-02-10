using Blackset.Data.Items.Types;
using UnityEngine;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Реестр типов расходников
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Registries/" + nameof(ConsumableTypeRegistry),
        fileName = nameof(ConsumableTypeRegistry))]
    public sealed class ConsumableTypeRegistry : BaseDataRegistry<ConsumableType> { }
}