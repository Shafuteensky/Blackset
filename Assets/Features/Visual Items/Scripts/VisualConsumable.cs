using Blackset.Data.Items.Visual.Modules;
using Blackset.Duel.Participants;
using Blackset.Inventories;
using Blackset.Inventories.Scripts.Items;
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
        // [SerializeField] private DiceMeshView meshView; // TODO: добавить при реализации

        protected override ItemClass visualItemClass => ItemClass.Consumable;
        
        public override void Initialize(string itemId, string ownerParticipantId,
            Inventory inventory, string newItemCellId)
        {
            base.Initialize(itemId, ownerParticipantId, inventory, newItemCellId);
            if (duelController == null) return;

            DuelParticipantState participant = duelController.DuelContext.Participants[ownerParticipantId];
            if (participant.Sets.TryGetConsumable(itemId, out ConsumableItemContext consumableItem))
                viewRepresentation.Initialize(consumableItem);
            else
                ServiceDebug.LogError($"Ошибка инициализации расходника {itemId}");
        }
    }
}