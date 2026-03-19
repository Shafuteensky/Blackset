using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Extensions.Events;
using Extensions.Generics;
using UnityEngine;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Кнопка запроса паса в ходу
    /// </summary>
    public sealed class DuelPassHoldButton : AbstractHoldButton
    {
        private EventHub eventHub;
        private IDuelInputHandler inputHandler;
        
        protected override void Awake()
        {
            base.Awake();
            eventHub = DuelController.Instance.EventHub;
        }
        
        private void Start()
        {
            inputHandler = InputRegistrar.Instance.InputHandler;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (eventHub == null) return;
            
            eventHub.Subscribe<DeclarationStartedEvent>(OnDeclarationStarted);
            eventHub.Subscribe<PlanningStartedEvent>(OnPlanningStarted);
            eventHub.Subscribe<PlanningCompletedEvent>(OnPlanningCompleted);
        }

        protected override void OnDisable()
        {
            base.OnEnable();
            
            if (eventHub == null) return;

            eventHub.Unsubscribe<DeclarationStartedEvent>(OnDeclarationStarted);
            eventHub.Unsubscribe<PlanningStartedEvent>(OnPlanningStarted);
            eventHub.Unsubscribe<PlanningCompletedEvent>(OnPlanningCompleted);
        }
        
        public override void OnButtonClick()
        {
            // Запрос пасса 
            inputHandler?.OnPassRequested();
        }

        #region Изменение состояния кнопки

        private void OnDeclarationStarted(DeclarationStartedEvent evt)
        {
            SetInteractable(true);
        }

        private void OnPlanningStarted(PlanningStartedEvent evt)
        {
            SetInteractable(true);
        }

        private void OnPlanningCompleted(PlanningCompletedEvent evt)
        {
            SetInteractable(false);
        }

        private void SetInteractable(bool value)
        {
            if (button == null) return;

            button.interactable = value;
        }
        
        #endregion
    }
}