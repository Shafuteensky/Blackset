using UnityEngine;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Реестр расходуемых предметов
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Registries/" + nameof(ConsumableDataRegistry),
        fileName = nameof(ConsumableDataRegistry))]
    public sealed class ConsumableDataRegistry : BaseDataRegistry<ConsumableData> { }
}