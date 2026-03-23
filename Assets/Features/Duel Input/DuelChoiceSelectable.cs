using Blackset.Data.Items.Visual;
using Extensions.Log;
using Blackset.Inventories.Scripts.Items;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Выбор предмета в фазах планирования
    /// </summary>
    [RequireComponent(typeof(BaseVisual))]
    public sealed class DuelChoiceSelectable : InputHandlerProvider, IPointerClickHandler
    {
        [SerializeField] private ItemClass choiceType;

        private BaseVisual baseVisual;

        private void Awake()
        {
            baseVisual = GetComponent<BaseVisual>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SelectionState selection = new SelectionState(baseVisual.ItemId);

            switch (choiceType)
            {
                case ItemClass.Dice:
                {
                    inputHandler.OnDiceSelected(baseVisual.OwnerParticipantId, selection);
                    break;
                }
                case ItemClass.Consumable:
                {
                    inputHandler.OnConsumableSelected(baseVisual.OwnerParticipantId, selection);
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