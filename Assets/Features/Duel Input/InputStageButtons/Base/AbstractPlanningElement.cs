using Blackset.Duel;
using Blackset.Duel.Participants;
using Blackset.Duel.Sequence;
using UnityEngine;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Базовый элемент окна планирования
    /// </summary>
    public abstract class AbstractPlanningElement : MonoBehaviour
    {
        protected IDuelInputHandler inputHandler;
        protected TurnParticipantState turnState;
        protected PlanningStageType currentStage = PlanningStageType.None;

        protected virtual void Start()
        {
            if (DuelController.Instance == null) return;

            inputHandler = InputRegistrar.Instance.InputHandler;
            RefreshVisualState();
            RefreshInteractable();
        }

        /// <summary>
        /// Задать состояние хода игрока
        /// </summary>
        public virtual void SetTurnState(TurnParticipantState state)
        {
            turnState = state;
            RefreshVisualState();
            RefreshInteractable();
        }

        /// <summary>
        /// Задать текущий этап
        /// </summary>
        public virtual void SetStage(PlanningStageType stage)
        {
            currentStage = stage;
            RefreshVisualState();
            RefreshInteractable();
        }

        /// <summary>
        /// Сбросить состояние элемента
        /// </summary>
        public virtual void ResetState()
        {
            currentStage = PlanningStageType.None;
            SetInteractable(false);
            RefreshVisualState();
        }

        /// <summary>
        /// Установить доступность UI-элемента
        /// </summary>
        protected abstract void SetInteractable(bool state);

        /// <summary>
        /// Обновить визуальное состояние элемента
        /// </summary>
        protected virtual void RefreshVisualState() { }

        /// <summary>
        /// Обновить доступность элемента
        /// </summary>
        public abstract void RefreshInteractable();
    }
}