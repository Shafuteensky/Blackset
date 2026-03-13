using Blackset.Duel.Sequence;
using Blackset.DuelEvents;
using Blackset.DuelEvents.EventTypes;
using Extensions.Log;
using UnityEngine;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Управление UI-панелями фаз хода битвы дуэли
    /// </summary>
    public sealed class RollPlanningPhasePanelsPresenter : MonoBehaviour
    {
        [Header("Панели-индикаторы фаз хода"), Space]
        [SerializeField] private GameObject declarationPanel;
        [SerializeField] private GameObject planningPanel;
        [SerializeField] private GameObject processingPanel;

        private EventHub eventHub;

        private void OnEnable()
        {
            DuelController duelController = DuelController.Instance;
            
            if (duelController != null)
            {
                eventHub = duelController.EventHub;
                if (eventHub == null)
                {
                    ServiceDebug.LogError("Ошибка инициализации");
                    return;
                }
                
                Subscribe();
                HideAll();
            }
        }
        
        private void OnDisable()
        {
            Unsubscribe();
            HideAll();
        }

        private void Subscribe()
        {
            if (eventHub == null) return;

            eventHub.Subscribe<DeclarationStartedEvent>(OnDeclarationStarted);
            eventHub.Subscribe<PlanningStartedEvent>(OnPlanningStarted);
            eventHub.Subscribe<PlanningCompletedEvent>(OnPlanningCompleted);
        }

        private void Unsubscribe()
        {
            if (eventHub == null) return;

            eventHub.Unsubscribe<DeclarationStartedEvent>(OnDeclarationStarted);
            eventHub.Unsubscribe<PlanningStartedEvent>(OnPlanningStarted);
            eventHub.Unsubscribe<PlanningCompletedEvent>(OnPlanningCompleted);
        }

        private void OnDeclarationStarted(DeclarationStartedEvent evt) => SetPanels(true, false, false);

        private void OnPlanningStarted(PlanningStartedEvent evt) => SetPanels(false, true, false);

        private void OnPlanningCompleted(PlanningCompletedEvent evt) => SetPanels(false, false, true);

        private void HideAll() => SetPanels(false, false, false);

        private void SetPanels(bool declaration, bool planning, bool processing)
        {
            if (declarationPanel != null) declarationPanel.SetActive(declaration);
            if (planningPanel != null) planningPanel.SetActive(planning);
            if (processingPanel != null) processingPanel.SetActive(processing);
        }
    }
}