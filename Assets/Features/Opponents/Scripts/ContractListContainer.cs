using Extensions.Data.InMemoryData;
using UnityEngine;

namespace Blackset.Opponents
{
    /// <summary>
    /// Хранилище доступных соперников
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ContractListContainer),
        menuName = "Blackset/Opponents/" + nameof(ContractListContainer))]
    public class ContractListContainer : InMemoryDataContainer<OpponentContract> { }
}