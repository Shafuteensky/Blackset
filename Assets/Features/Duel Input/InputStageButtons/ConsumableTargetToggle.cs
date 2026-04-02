using Blackset.Duel;
using Blackset.Duel.Sequence;
using UnityEngine;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Переключатель цели применения расходника.
    /// Включен  = цель на себя.
    /// Выключен = цель на соперника.
    /// </summary>
    public sealed class ConsumableTargetToggle : AbstractPlanningToggle
    {
        [Tooltip("Родительский элемент")]
        [SerializeField] private GameObject parentElement;
        
        public override void OnToggled(bool state)
        {
            if (inputHandler == null) return;

            DuelController duelController = DuelController.Instance;
            if (duelController == null || duelController.DuelContext == null) return;

            string playerId = duelController.DuelContext.PlayerId;
            string targetParticipantId = state
                ? duelController.DuelContext.PlayerId
                : duelController.DuelContext.OpponentId;

            inputHandler.OnConsumableTargetSelected(playerId, targetParticipantId);
        }

        public override void RefreshInteractable()
        {
            bool shouldBeVisible = ShouldBeVisible();
            if (parentElement != null)
                parentElement.SetActive(shouldBeVisible);
            else
                gameObject.SetActive(shouldBeVisible);

            if (!shouldBeVisible)
                return;

            toggle.interactable = inputHandler != null &&
                                  turnState != null &&
                                  currentStage == PlanningStageType.Selection &&
                                  !turnState.HasPassed.Value;
        }

        protected override void RefreshVisualState()
        {
            bool shouldBeVisible = ShouldBeVisible();
            if (parentElement != null)
                parentElement.SetActive(shouldBeVisible);
            else
                gameObject.SetActive(shouldBeVisible);

            if (!shouldBeVisible || toggle == null)
                return;

            DuelController duelController = DuelController.Instance;
            if (duelController == null || duelController.DuelContext == null || turnState == null)
                return;

            string playerId = duelController.DuelContext.PlayerId;
            bool targetSelf = turnState.SelectedTargetParticipantId.Value == playerId;

            toggle.SetIsOnWithoutNotify(targetSelf);
        }

        private bool ShouldBeVisible()
        {
            return turnState != null &&
                   currentStage == PlanningStageType.Selection &&
                   !turnState.HasPassed.Value &&
                   turnState.IsConsumableChosen.Value;
        }
    }
}