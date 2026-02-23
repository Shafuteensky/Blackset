using Blackset.Data.Registries;
using UnityEngine;

namespace Blackset.Opponents
{
    /// <summary>
    /// Реестр данных о соперниках
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(OpponentsRegistry),
        menuName = "Blackset/Opponents/" + nameof(OpponentsRegistry))]
    public class OpponentsRegistry : BaseDataRegistry<OpponentData> { }
}