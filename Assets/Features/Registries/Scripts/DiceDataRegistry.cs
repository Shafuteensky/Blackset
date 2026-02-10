using UnityEngine;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Реестр данных костей
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Registries/" + nameof(DiceDataRegistry),
        fileName = nameof(DiceDataRegistry))]
    public sealed class DiceDataRegistry : BaseDataRegistry<DiceData> { }
}