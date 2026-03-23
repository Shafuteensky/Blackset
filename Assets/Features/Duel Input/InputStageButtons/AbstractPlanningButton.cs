using Blackset.Duel;
using Blackset.Duel.Participants;
using Blackset.Duel.Sequence;
using Extensions.Generics;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Базовая кнопка для окна планирования
    /// </summary>
    public abstract class AbstractPlanningButton : AbstractButton
    {
        protected IDuelInputHandler inputHandler;
        protected TurnParticipantState turnState;
        protected PlanningStageType currentStage = PlanningStageType.None;

        protected override void Awake()
        {
            base.Awake();
            button.interactable = false;
        }

        protected virtual void Start()
        {
            if (DuelController.Instance == null) return;

            inputHandler = InputRegistrar.Instance.InputHandler;
        }

        /// <summary>
        /// Задать состояние хода игрока
        /// </summary>
        public virtual void SetTurnState(TurnParticipantState state)
        {
            turnState = state;
            RefreshInteractable();
        }

        /// <summary>
        /// Задать текущий этап
        /// </summary>
        public virtual void SetStage(PlanningStageType stage)
        {
            currentStage = stage;
            RefreshInteractable();
        }

        /// <summary>
        /// Сбросить состояние кнопки
        /// </summary>
        public virtual void ResetState()
        {
            currentStage = PlanningStageType.None;
            button.interactable = false;
        }

        /// <summary>
        /// Обновить доступность кнопки
        /// </summary>
        public abstract void RefreshInteractable();
    }
}