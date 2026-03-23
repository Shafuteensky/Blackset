using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Запрет интеракции с визуальным предметом, не принадлежащим игроку
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VisualItemInteractionBlocker : BaseVisualItemModule
    {
        /// <summary>
        /// Состояние доступности к интеракции
        /// </summary>
        public bool IsInteractable { get; private set; }

        private Collider triggerCollider;

        private void Awake() => triggerCollider = GetComponent<Collider>();

        // TODO Дополнить логику: изменять в зависимости от раскрытия KnowledgeState
        public override void Initialize(VisualItemContext context)
        {
            IsInteractable = context.IsPlayer;
            triggerCollider.enabled = context.IsPlayer;
        }
    }
}