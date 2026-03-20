using Blackset.Data.Items.Visual.Modules;
using Blackset.Duel.Participants;
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
        [Header("Модули (дайс)"), Space]
        [SerializeField] private DiceResultView resultView;
        [SerializeField] private DiceViewRepresentation viewRepresentation;
        // [SerializeField] private DiceMeshView meshView; // TODO: добавить при реализации

        public override void Initialize(string itemId, string ownerParticipantId,
            Inventory inventory, string newItemCellId)
        {
            base.Initialize(itemId, ownerParticipantId, inventory, newItemCellId);
            if (duelController == null) return;
            
            resultView.Initialize(duelController, itemId, ownerParticipantId);

            DuelParticipantState participant = duelController.DuelContext.Participants[ownerParticipantId];
            if (participant.Sets.TryGetDice(itemId, out DiceItemContext diceItem))
                viewRepresentation.Initialize(diceItem);
            else
                ServiceDebug.LogError($"Ошибка инициализации дайса {itemId}");
        }
    }
}