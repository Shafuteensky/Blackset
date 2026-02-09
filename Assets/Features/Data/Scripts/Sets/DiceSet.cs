using UnityEngine;

namespace Blackset.Data
{
    /// <summary>
    /// Набор дайсов
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Sets/" + nameof(DiceSet),
        fileName = nameof(DiceSet))]
    public sealed class DiceSet : BaseSet { }
}