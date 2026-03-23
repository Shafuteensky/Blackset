using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Запрет интеракции с визуальным предметом, не принадлежащим игроку
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VisualItemInteractionBlocker : BaseKnownStateCallback
    {
        /// <summary>
        /// Состояние доступности к интеракции
        /// </summary>
        public bool IsInteractable { get; private set; }

        private Collider triggerCollider;

        private void Awake() => triggerCollider = GetComponent<Collider>();

        public override void Initialize(VisualItemContext context)
        {
            base.Initialize(context);
            
            IsInteractable = context.IsPlayer;
            triggerCollider.enabled = context.IsPlayer;
        }

        protected override void OnStateUpdate()
        {
            if (IsInteractable) return;
            triggerCollider.enabled = IsItemRevealed();
        }
    }
}