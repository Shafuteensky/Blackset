using Blackset.Data.Items.Visual.Modules;
using Blackset.Duel.Participants;
using Blackset.Inventories;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Data.Items.Visual
{
    /// <summary>
    /// Визуальный префаб расходника
    /// </summary>
    public sealed class VisualConsumable : BaseVisual
    {
        [Header("Модули (расходник)"), Space]
        [SerializeField] private ConsumableViewRepresentation viewRepresentation;
        [SerializeField] private SelectedConsumableIndicator selectionView;
        // [SerializeField] private DiceMeshView meshView; // TODO: добавить при реализации

        public override void Initialize(string itemId, string ownerParticipantId,
            Inventory inventory, string newItemCellId)
        {
            base.Initialize(itemId, ownerParticipantId, inventory, newItemCellId);
            if (duelController == null) return;
            
            selectionView.Initialize(duelController, itemId, ownerParticipantId);

            DuelParticipantState participant = duelController.DuelContext.Participants[ownerParticipantId];
            if (participant.Sets.TryGetConsumable(itemId, out ConsumableItemContext consumableItem))
                viewRepresentation.Initialize(consumableItem);
            else
                ServiceDebug.LogError($"Ошибка инициализации расходника {itemId}");
        }
    }
}