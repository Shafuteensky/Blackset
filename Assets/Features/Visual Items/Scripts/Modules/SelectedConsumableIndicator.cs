using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Индикатор выбора расходника
    /// </summary>
    public class SelectedConsumableIndicator : MonoBehaviour
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

        /// <summary>
        /// Инициализация от координатора VisualDice
        /// </summary>
        public void Initialize(DuelController duelController, string itemId, string ownerParticipantId)
        {
            this.itemId = itemId;
            this.ownerParticipantId = ownerParticipantId;
            this.duelController = duelController;

            if (duelController.DuelContext.Participants[ownerParticipantId].IsPlayer)
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