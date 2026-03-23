using Blackset.Duel.Participants;
using Blackset.Duel.Sequence;
using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Индикатор объявления и выбора дайса
    /// </summary>
    public class SelectedDiceIndicator : BaseVisualItemModule
    {
        [Header("Элементы"), Space]
        [Tooltip("Индикатор объявления")]
        [SerializeField] private GameObject declaredIndicator;
        [Tooltip("Индикатор выбора")]
        [SerializeField] private GameObject selectedIndicator;

        private string itemId;
        private string ownerParticipantId;
        private DuelController duelController;
        private TurnParticipantState turnState;

        private void Awake() => HideIndicators();

        private void OnDestroy()
        {
            if (turnState != null)
            {
                turnState.DeclaredDice.Unsubscribe(OnDeclaredChanged);
                turnState.SelectedDice.Unsubscribe(OnSelectedChanged);
            }
        }

        public override void Initialize(VisualItemContext context)
        {
            itemId = context.ItemId;
            ownerParticipantId = context.OwnerParticipantId;
            duelController = context.DuelController;

            if (duelController == null || duelController.DuelContext == null)
                return;

            if (!duelController.DuelContext.Participants.TryGetValue(ownerParticipantId, out var participant))
                return;

            turnState = participant.FightState.TurnState;

            turnState.DeclaredDice.Subscribe(OnDeclaredChanged, true);
            if (ownerParticipantId == duelController.DuelContext.PlayerId)
                turnState.SelectedDice.Subscribe(OnSelectedChanged, true);
        }

        private void OnDeclaredChanged(string declaredId)
        {
            if (declaredIndicator != null)
                declaredIndicator.SetActive(declaredId == itemId);
        }

        private void OnSelectedChanged(string selectedId)
        {
            if (selectedIndicator != null)
                selectedIndicator.SetActive(selectedId == itemId);
        }

        private void HideIndicators()
        {
            if (declaredIndicator != null)
                declaredIndicator.SetActive(false);

            if (selectedIndicator != null)
                selectedIndicator.SetActive(false);
        }
    }
}