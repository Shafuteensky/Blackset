using Blackset.Duel;
using Blackset.Duel.Sequence;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Переключатель цели применения расходника.
    /// </summary>
    public sealed class ConsumableTargetToggle : AbstractPlanningToggle
    {
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
            toggle.interactable = inputHandler != null &&
                                  turnState != null &&
                                  currentStage == PlanningStageType.Selection &&
                                  !turnState.HasPassed.Value;
        }

        protected override void RefreshVisualState()
        {
            if (toggle == null)
                return;

            DuelController duelController = DuelController.Instance;
            if (duelController == null || duelController.DuelContext == null || turnState == null)
                return;

            string playerId = duelController.DuelContext.PlayerId;
            bool targetSelf = turnState.SelectedTargetParticipantId.Value == playerId;

            toggle.SetIsOnWithoutNotify(targetSelf);
        }
    }
}