using System;
using Blackset.Data.Registries;
using Blackset.Player;
using Extensions.Log;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blackset.Storms
{
    /// <summary>
    /// Контроллер шторма дуэли
    /// </summary>
    public sealed class StormController : PlayerProgressUpdater
    {
        /// <summary>
        /// Появление нового шторма
        /// </summary>
        public event Action<string> onNewStorm;

        private const float DEFAULT_STORM_CHANCE = 0.5f;

        [Header("Активный шторм"), Space]
        [SerializeField]
        [Tooltip("Вероятность появления шторма после сыгранной дуэли")]
        [Range(0f, 1f)]
        private float stormChance = DEFAULT_STORM_CHANCE;
        [SerializeField]
        private ActiveStorm activeStorm;

        private void Awake()
        {
            ServiceGuard.NotNull(activeStorm, nameof(activeStorm));

            if (!activeStorm.HasSavedState()) UpdateStorm();
        }

        private void Start()
        {
            if (IsUpdateNeeded()) UpdateStorm();
        }

        private void UpdateStorm()
        {
            if (stormChance > Random.Range(0f, 1f))
            {
                Storm randomStorm = GameData.Instance.Storms.GetRandom();
                activeStorm.Set(randomStorm.Id);
                onNewStorm?.Invoke(randomStorm.Id);
            }
            else
            {
                activeStorm.Clear();
                onNewStorm?.Invoke(string.Empty);
            }
        }
    }
}