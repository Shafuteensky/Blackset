using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Индикатор объявления и выбора дайса
    /// </summary>
    public class SelectedDiceIndicator : MonoBehaviour
    {
        [Header("Элементы"), Space]
        [Tooltip("Индикатор объявления")]
        [SerializeField] private GameObject declaredIndicator;
        [Tooltip("Индикатор выбора")]
        [SerializeField] private GameObject selectedIndicator;

        private string itemId;
        private string ownerParticipantId;
        private DuelController duelController;

        private void Awake() => HideIndicators(new());
        
        private void OnDestroy()
        {
            if (duelController == null) return;
            
            duelController.EventHub.Unsubscribe<DeclaredDiceEvent>(ShowDeclared);
            duelController.EventHub.Unsubscribe<SelectedDiceEvent>(ShowSelection);
            duelController.EventHub.Unsubscribe<PlanningCompletedEvent>(HideIndicators);
        }

        /// <summary>
        /// Инициализация от координатора VisualDice
        /// </summary>
        public void Initialize(DuelController duelController, string itemId, string ownerParticipantId)
        {
            this.itemId = itemId;
            this.ownerParticipantId = ownerParticipantId;
            this.duelController = duelController;

            duelController.EventHub.Subscribe<DeclaredDiceEvent>(ShowDeclared);
            if (duelController.DuelContext.Participants[ownerParticipantId].IsPlayer)
                duelController.EventHub.Subscribe<SelectedDiceEvent>(ShowSelection);
            duelController.EventHub.Subscribe<PlanningCompletedEvent>(HideIndicators);
        }

        private void ShowDeclared(DeclaredDiceEvent handler)
        {
            if (declaredIndicator == null) return;
            
            if (handler.ParticipantOwnerId != ownerParticipantId ||
                handler.DiceId != itemId) return;
            
            declaredIndicator.SetActive(true);
        }
        
        private void ShowSelection(SelectedDiceEvent handler)
        {
            if (selectedIndicator == null) return;
            
            if (handler.ParticipantOwnerId != ownerParticipantId ||
                handler.DiceId != itemId) return;
            
            selectedIndicator.SetActive(true);
        }

        private void HideIndicators(PlanningCompletedEvent _)
        {
            if (selectedIndicator != null) selectedIndicator.SetActive(false);
            if (declaredIndicator != null) declaredIndicator.SetActive(false);
        }
    }
}