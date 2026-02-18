using System;
using Extensions.Data.InMemoryData;
using UnityEngine;

namespace Blackset.Player
{
    /// <summary>
    /// Контейнер данных прогресса игрока
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlayerProgressDataContainer),
        menuName = "Blackset/Player/" + nameof(PlayerProgressDataContainer))]
    public class PlayerProgressDataContainer : InMemorySingleDataContainer<PlayerProgressData>
    {
        /// <summary>
        /// Засчитать сыгранную дуэль
        /// </summary>
        public void DuelPlayed()
        {
            Data.DuelPlayed();
            MarkDirty();
        }
    }
}