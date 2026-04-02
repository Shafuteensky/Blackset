using Blackset.Duel;
using Blackset.Duel.Participants;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using UnityEngine;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Контроллер панелей окна планирования хода
    /// </summary>
    public sealed class RollPlanningElementsController : MonoBehaviour
    {
        [Header("Элементы планирования"), Space]
        [SerializeField] private AbstractPlanningElement[] planningElements;

        private DuelController duelController;
        private TurnParticipantState playerTurnState;
        private PlanningStageType currentStage = PlanningStageType.None;
        private bool isInitialized;

        private void Awake()
        {
            if (planningElements == null || planningElements.Length == 0)
                planningElements = GetComponentsInChildren<AbstractPlanningElement>(true);
        }

        private void OnEnable()
        {
            duelController = DuelController.Instance;
            if (duelController == null) return;

            duelController.EventHub.Subscribe<DuelInitedEvent>(OnDuelInited);
            duelController.EventHub.Subscribe<DeclarationStartedEvent>(OnDeclarationStarted);
            duelController.EventHub.Subscribe<SelectionStartedEvent>(OnSelectionStarted);
            duelController.EventHub.Subscribe<DuelFinishEvent>(OnDuelFinished);
        }

        private void OnDisable()
        {
            UnbindTurnState();

            if (duelController == null) return;

            duelController.EventHub.Unsubscribe<DuelInitedEvent>(OnDuelInited);
            duelController.EventHub.Unsubscribe<DeclarationStartedEvent>(OnDeclarationStarted);
            duelController.EventHub.Unsubscribe<SelectionStartedEvent>(OnSelectionStarted);
            duelController.EventHub.Unsubscribe<DuelFinishEvent>(OnDuelFinished);
        }

        /// <summary>
        /// Показать панель объявления
        /// </summary>
        public void ShowDeclaration()
        {
            currentStage = PlanningStageType.Declaration;
            ApplyStateToElements();
            RefreshElements();
        }

        /// <summary>
        /// Показать панель выбора
        /// </summary>
        public void ShowSelection()
        {
            currentStage = PlanningStageType.Selection;
            ApplyStateToElements();
            RefreshElements();
        }

        private void OnDuelInited(DuelInitedEvent _)
        {
            isInitialized = true;
            BindTurnState();
            TryRestoreCurrentView();
        }

        private void OnDeclarationStarted(DeclarationStartedEvent _)
        {
            if (!isInitialized) return;
            ShowDeclaration();
        }

        private void OnSelectionStarted(SelectionStartedEvent _)
        {
            if (!isInitialized) return;
            ShowSelection();
        }

        private void OnDuelFinished(DuelFinishEvent _)
        {
            ResetElements();
            UnbindTurnState();
            isInitialized = false;
        }

        private void BindTurnState()
        {
            if (duelController?.DuelContext == null) return;
            if (!duelController.DuelContext.Participants.TryGetValue(
                    duelController.DuelContext.PlayerId, out DuelParticipantState participant)) return;

            playerTurnState = participant.FightState.TurnState;

            playerTurnState.IsDiceDeclared.Subscribe(OnReactiveChanged, true);
            playerTurnState.IsDiceChosen.Subscribe(OnReactiveChanged, true);
            playerTurnState.HasPassed.Subscribe(OnHasPassedChanged, true);

            ApplyStateToElements();
        }

        private void UnbindTurnState()
        {
            if (playerTurnState == null) return;

            playerTurnState.IsDiceDeclared.Unsubscribe(OnReactiveChanged);
            playerTurnState.IsDiceChosen.Unsubscribe(OnReactiveChanged);
            playerTurnState.HasPassed.Unsubscribe(OnHasPassedChanged);

            playerTurnState = null;
        }

        private void ApplyStateToElements()
        {
            if (planningElements == null) return;

            foreach (AbstractPlanningElement element in planningElements)
            {
                if (element == null) continue;
                element.SetTurnState(playerTurnState);
                element.SetStage(currentStage);
            }
        }

        private void RefreshElements()
        {
            if (planningElements == null) return;

            foreach (AbstractPlanningElement element in planningElements)
                element?.RefreshInteractable();
        }

        private void ResetElements()
        {
            if (planningElements == null) return;

            foreach (AbstractPlanningElement element in planningElements)
                element?.ResetState();
        }

        private void TryRestoreCurrentView()
        {
            if (!isInitialized || playerTurnState == null || playerTurnState.HasPassed.Value)
                return;

            if (playerTurnState.IsDiceDeclared.Value)
                ShowSelection();
            else
                ShowDeclaration();
        }

        private void OnReactiveChanged(bool _)
        {
            if (currentStage != PlanningStageType.None)
                RefreshElements();
        }

        private void OnHasPassedChanged(bool passed)
        {
            if (!passed && currentStage != PlanningStageType.None)
                RefreshElements();
        }
    }
}