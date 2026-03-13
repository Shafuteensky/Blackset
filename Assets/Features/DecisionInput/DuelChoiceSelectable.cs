using Blackset.Data.Items.Visual;
using Blackset.Duel.Targets;
using Extensions.Log;
using Features.Inventory.Scripts.Items;
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
                    DuelSelectionBus.PublishDiceSelected(
                        baseVisual.OwnerParticipantId,
                        baseVisual.ItemId);
                    break;
                }
                case ItemClass.Consumable:
                {
                    DuelSelectionBus.PublishConsumableSelected(
                        baseVisual.OwnerParticipantId,
                        baseVisual.ItemId,
                        ApplyTarget.Self); // TODO Заменить на выбор применения при необходимости (расходник или особый дайс)
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