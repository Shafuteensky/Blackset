using Blackset.Data.Items.Visual;
using Blackset.Data.Items.Visual.Modules;
using Extensions.Log;
using Blackset.Inventories.Scripts.Items;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Выбор предмета в фазах планирования
    /// </summary>
    public sealed class DuelChoiceSelectable : BaseVisualItemModule, IPointerClickHandler
    {
        [SerializeField] private ItemClass choiceType;

        private IDuelInputHandler inputHandler;
        private VisualItemContext visualItemContext;
        
        private void Start() => inputHandler = InputRegistrar.Instance.InputHandler;

        public override void Initialize(VisualItemContext context)
        {
            visualItemContext = context;
            
            string playerId = visualItemContext.DuelController.DuelContext.PlayerId;
            if (visualItemContext.OwnerParticipantId != playerId) enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SelectionState selection = new SelectionState(visualItemContext.ItemId);

            switch (choiceType)
            {
                case ItemClass.Dice:
                {
                    inputHandler.OnDiceSelected(visualItemContext.OwnerParticipantId, selection);
                    break;
                }
                case ItemClass.Consumable:
                {
                    inputHandler.OnConsumableSelected(visualItemContext.OwnerParticipantId, selection);
                    break;
                }
                default:
                {
                    ServiceDebug.LogError($"Необработанный случай выбора {nameof(choiceType)}: {choiceType}");
                    break;
                }
            }
        }
    }
}