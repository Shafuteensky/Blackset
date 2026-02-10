using Blackset.Data.Items.Types;
using UnityEngine;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Реестр типов дайсов
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Registries/" + nameof(DiceTypeRegistry),
        fileName = nameof(DiceTypeRegistry))]
    public sealed class DiceTypeRegistry : BaseDataRegistry<DiceType> { }
}