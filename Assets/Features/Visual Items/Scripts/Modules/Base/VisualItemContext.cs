using Blackset.Duel.Sequence;
using Blackset.Inventories.Scripts.Items;
using UnityEngine;

namespace Blackset.Data.Items.Visual
{
    /// <summary>
    /// Общий контекст данных визуального предмета, передаваемый всем модулям
    /// </summary>
    public readonly struct VisualItemContext
    {
        public readonly DuelController DuelController;
        public readonly string ItemId;
        public readonly ItemClass ItemClass;
        public readonly string OwnerParticipantId;
        public readonly Transform Transform;
        
        public readonly bool IsPlayer;

        public VisualItemContext(
            DuelController duelController,
            string itemId,
            ItemClass itemClass,
            string ownerParticipantId,
            Transform transform)
        {
            DuelController = duelController;
            ItemId = itemId;
            ItemClass = itemClass;
            OwnerParticipantId = ownerParticipantId;
            Transform = transform;
            
            IsPlayer = DuelController.DuelContext.Participants[ownerParticipantId].IsPlayer;
        }
    }
}