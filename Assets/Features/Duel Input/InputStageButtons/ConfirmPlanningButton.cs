using Blackset.Duel;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Кнопка подтверждения этапа планирования
    /// </summary>
    public sealed class ConfirmPlanningButton : AbstractPlanningButton
    {
        /// <summary>
        /// Выполнить действие по кнопке
        /// </summary>
        public override void OnButtonClick()
        {
            if (inputHandler == null || !button.interactable)
                return;

            inputHandler.OnInputCompletion();
        }

        /// <summary>
        /// Обновить доступность кнопки
        /// </summary>
        public override void RefreshInteractable()
        {
            if (button == null) return;
            if (turnState == null)
            {
                button.interactable = false;
                return;
            }

            switch (currentStage)
            {
                case PlanningStageType.Declaration:
                    button.interactable = turnState.IsDiceDeclared.Value;
                    break;

                case PlanningStageType.Selection:
                    button.interactable = turnState.IsDiceChosen.Value;
                    break;

                default:
                    button.interactable = false;
                    break;
            }
        }
    }
}