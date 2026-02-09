using Extensions.Data.InMemoryData;
using UnityEngine;

namespace Blackset.Player
{
    /// <summary>
    /// Контейнер данных игрока
    /// </summary>
    [CreateAssetMenu(fileName = nameof(PlayerDataContainer), menuName = "Blackset/Player/" + nameof(PlayerDataContainer))]
    public class PlayerDataContainer : InMemorySingleDataContainer<PlayerData> { }
}