using Blackset.Data.Items.Visual.Modules;
using Blackset.Duel.Participants;
using Blackset.Duel.Sequence;
using Blackset.Inventories;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Data.Items.Visual
{
    /// <summary>
    /// Визуальный префаб дайса
    /// </summary>
    /// <remarks>
    /// Координатор: получает контекст от фабрики и передаёт его дочерним компонентам
    /// </remarks>
    public sealed class VisualDice : BaseVisual
    {
        [Header("Дочерние компоненты (модули)"), Space]
        [SerializeField] private DicePositionController positionController;
        [SerializeField] private DiceResultView resultView;
        [SerializeField] private VisualItemInteractionBlocker interactionBlocker;
        [SerializeField] private DiceViewRepresentation viewRepresentation;
        // [SerializeField] private DiceMeshView meshView; // TODO: добавить при реализации

        public override void Initialize(string itemId, string ownerParticipantId,
            Inventory inventory, string newItemCellId)
        {
            base.Initialize(itemId, ownerParticipantId, inventory, newItemCellId);

            DuelController duelController = DuelController.Instance;
            if (duelController == null)
            {
                ServiceDebug.LogError(
                    $"Не найден инстанс {nameof(DuelController)}, визуальный дайс не инициализирован");
                return;
            }

            positionController.Initialize(duelController, itemId, ownerParticipantId, transform);
            resultView.Initialize(duelController, itemId, ownerParticipantId);

            bool isParticipantPlayer = duelController.DuelContext.PlayerId == ownerParticipantId;
            interactionBlocker.Initialize(isParticipantPlayer);

            DuelParticipantState participant = duelController.DuelContext.Participants[ownerParticipantId];
            participant.Sets.TryGetDice(itemId, out DiceItemContext diceItem);
            viewRepresentation.Initialize(diceItem);
        }
    }
}