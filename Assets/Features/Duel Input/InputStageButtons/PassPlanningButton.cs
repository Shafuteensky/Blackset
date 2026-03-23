using Blackset.Duel;
using UnityEngine;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Кнопка паса в окне планирования
    /// </summary>
    public sealed class PassPlanningButton : AbstractPlanningButton
    {
        [SerializeField] private bool isAvailableOnDeclaration = true;
        [SerializeField] private bool isAvailableOnSelection = true;

        /// <summary>
        /// Выполнить действие по кнопке
        /// </summary>
        public override void OnButtonClick()
        {
            if (inputHandler == null || !button.interactable)
                return;

            inputHandler.OnPassRequested();
        }

        /// <summary>
        /// Обновить доступность кнопки
        /// </summary>
        public override void RefreshInteractable()
        {
            switch (currentStage)
            {
                case PlanningStageType.Declaration:
                    button.interactable = isAvailableOnDeclaration;
                    break;

                case PlanningStageType.Selection:
                    button.interactable = isAvailableOnSelection;
                    break;

                default:
                    button.interactable = false;
                    break;
            }
        }
    }
}