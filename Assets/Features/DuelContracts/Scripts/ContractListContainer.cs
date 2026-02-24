using Extensions.Data.InMemoryData;
using UnityEngine;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// Хранилище доступных соперников
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ContractListContainer),
        menuName = "Blackset/Opponents/" + nameof(ContractListContainer))]
    public class ContractListContainer : InMemoryDataContainer<DuelContract> { }
}