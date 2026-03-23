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
        [Header("Кнопки"), Space]
        [SerializeField] private AbstractPlanningButton[] planningButtons;

        private DuelController duelController;
        private TurnParticipantState playerTurnState;
        private PlanningStageType currentStage = PlanningStageType.None;
        private bool isInitialized;

        private void Awake()
        {
            if (planningButtons == null || planningButtons.Length == 0)
                planningButtons = GetComponentsInChildren<AbstractPlanningButton>(true);
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
            ApplyStateToButtons();
            RefreshButtons();
        }

        /// <summary>
        /// Показать панель выбора
        /// </summary>
        public void ShowSelection()
        {
            currentStage = PlanningStageType.Selection;
            ApplyStateToButtons();
            RefreshButtons();
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

            ApplyStateToButtons();
        }

        private void UnbindTurnState()
        {
            if (playerTurnState == null) return;

            playerTurnState.IsDiceDeclared.Unsubscribe(OnReactiveChanged);
            playerTurnState.IsDiceChosen.Unsubscribe(OnReactiveChanged);
            playerTurnState.HasPassed.Unsubscribe(OnHasPassedChanged);

            playerTurnState = null;
        }

        private void ApplyStateToButtons()
        {
            if (planningButtons == null) return;

            foreach (AbstractPlanningButton button in planningButtons)
            {
                if (button == null) continue;
                button.SetTurnState(playerTurnState);
                button.SetStage(currentStage);
            }
        }

        private void RefreshButtons()
        {
            if (planningButtons == null) return;

            foreach (AbstractPlanningButton button in planningButtons)
                button?.RefreshInteractable();
        }

        private void TryRestoreCurrentView()
        {
            if (!isInitialized || playerTurnState == null || playerTurnState.HasPassed.Value)
            {
                return;
            }

            // Если дайс уже объявлен — восстанавливаем стадию выбора
            if (playerTurnState.IsDiceDeclared.Value)
                ShowSelection();
            else
                ShowDeclaration();
        }

        private void OnReactiveChanged(bool _)
        {
            if (currentStage != PlanningStageType.None)
                RefreshButtons();
        }

        private void OnHasPassedChanged(bool passed)
        {
            if (!passed && currentStage != PlanningStageType.None)
                RefreshButtons();
        }
    }
}