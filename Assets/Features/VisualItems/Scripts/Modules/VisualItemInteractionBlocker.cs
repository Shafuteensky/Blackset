using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Заперт интеракции с визуальным предметом, не принадлежащем игроку
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VisualItemInteractionBlocker : MonoBehaviour
    {
        private Collider triggerCollider;

        private void Awake() => triggerCollider = GetComponent<Collider>();

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="isPlayer">Принадлежит ли предмет игроку</param>
        public void Initialize(bool isPlayer) => triggerCollider.enabled = isPlayer;
    }
}