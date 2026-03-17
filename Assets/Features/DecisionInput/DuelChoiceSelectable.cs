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
    public sealed class DuelChoiceSelectable : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ItemClass choiceType;

        private BaseVisual baseVisual;
        
        SelectionState selection = new SelectionState();

        private void Awake()
        {
            baseVisual = GetComponent<BaseVisual>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (choiceType)
            {
                case ItemClass.Dice:
                {
                    selection = new SelectionState(baseVisual.ItemId);
                    DuelSelectionBus.PublishDiceSelected(baseVisual.OwnerParticipantId, selection);
                    break;
                }
                case ItemClass.Consumable:
                {
                    selection = new SelectionState(baseVisual.ItemId);
                    DuelSelectionBus.PublishConsumableSelected(baseVisual.OwnerParticipantId, selection); 
                    // TODO Заменить на выбор применения при необходимости (расходник или особый дайс)
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