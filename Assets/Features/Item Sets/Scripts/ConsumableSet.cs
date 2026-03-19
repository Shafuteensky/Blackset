using UnityEngine;

namespace Blackset.Data
{
    /// <summary>
    /// Набор расходников
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Sets/" + nameof(ConsumableSet),
        fileName = nameof(ConsumableSet))]
    public sealed class ConsumableSet : BaseSet { }
}