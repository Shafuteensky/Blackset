using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Индикатор выбора расходника
    /// </summary>
    public class SelectedConsumableIndicator : BaseVisualItemModule
    {
        [Header("Элементы"), Space]
        [Tooltip("Индикатор выбора")]
        [SerializeField] private GameObject selectedIndicator;

        private string itemId;
        private string ownerParticipantId;
        private DuelController duelController;

        private void Awake() => HideIndicator(new());

        private void OnDestroy()
        {
            if (duelController == null) return;

            duelController.EventHub.Unsubscribe<SelectedConsumableEvent>(ShowSelection);
            duelController.EventHub.Unsubscribe<PlanningCompletedEvent>(HideIndicator);
        }

        public override void Initialize(VisualItemContext context)
        {
            itemId = context.ItemId;
            ownerParticipantId = context.OwnerParticipantId;
            duelController = context.DuelController;

            if (context.IsPlayer)
                duelController.EventHub.Subscribe<SelectedConsumableEvent>(ShowSelection);
            duelController.EventHub.Subscribe<PlanningCompletedEvent>(HideIndicator);
        }

        private void ShowSelection(SelectedConsumableEvent handler)
        {
            if (selectedIndicator == null) return;

            if (handler.ParticipantOwnerId != ownerParticipantId ||
                handler.ConsumableId != itemId) return;

            selectedIndicator.SetActive(true);
        }

        private void HideIndicator(PlanningCompletedEvent _)
        {
            if (selectedIndicator != null) selectedIndicator.SetActive(false);
        }
    }
}