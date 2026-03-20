using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Заперт интеракции с визуальным предметом, не принадлежащем игроку
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VisualItemInteractionBlocker : MonoBehaviour
    {
        /// <summary>
        /// Состояние доступности к интеракции
        /// </summary>
        public bool IsInteractable { get; private set; }
        
        private Collider triggerCollider;
        
        private void Awake() => triggerCollider = GetComponent<Collider>();

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="isPlayer">Принадлежит ли предмет игроку</param>
        // TODO Дополнить логику: изменять в зависимости от раскрытия KnowledgeState
        public void Initialize(bool isPlayer) 
        {
            IsInteractable = isPlayer;
            triggerCollider.enabled = isPlayer;
        }
    }
}