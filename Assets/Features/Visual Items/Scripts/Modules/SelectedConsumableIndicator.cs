using System;
using Blackset.Duel.Participants;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Индикатор выбора расходника
    /// </summary>
    public class SelectedConsumableIndicator : BaseVisualItemModule
    {
        [Header("Элементы"), Space]
        [Tooltip("Индикатор выбора")]
        [SerializeField] private GameObject selectedIndicator;

        private string itemId;
        private TurnParticipantState turnState;
        private DuelController duelController;
        private string ownerParticipantId;

        private void Awake() => OnSelectedConsumableChanged(String.Empty);

        private void OnDestroy()
        {
            turnState?.SelectedConsumable.Unsubscribe(OnSelectedConsumableChanged);
        }

        public override void Initialize(VisualItemContext context)
        {
            itemId = context.ItemId;
            ownerParticipantId = context.OwnerParticipantId;
            duelController = context.DuelController;

            if (duelController.DuelContext.Participants.TryGetValue(ownerParticipantId, out var participant))
            {
                if (ownerParticipantId == duelController.DuelContext.PlayerId)
                {
                    turnState = participant.FightState.TurnState;
                    turnState.SelectedConsumable.Subscribe(OnSelectedConsumableChanged, true);
                }
            }
        }

        private void OnSelectedConsumableChanged(string selectedId)
        {
            if (selectedIndicator != null)
                selectedIndicator.SetActive(selectedId == itemId);
        }
    }
}