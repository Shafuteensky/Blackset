using Blackset.Opponents;
using UnityEngine;

namespace Blackset.OpponentsRestrictions
{
    /// <summary>
    /// Абстракция ограничения доступности соперника
    /// </summary>
    public abstract class OpponentRestriction : ScriptableObject
    {
        [Header("Блокировка доступа"), Space]
        [Tooltip("Какой доступ будет отключен при неисполнении условий")]
        [SerializeField] protected OpponentAvailability blockedAvailability = OpponentAvailability.All;

        /// <summary>
        /// Получить флаги доступности, которые нужно заблокировать
        /// </summary>
        public abstract OpponentAvailability GetBlockedAvailability(OpponentData opponent);
    }
}