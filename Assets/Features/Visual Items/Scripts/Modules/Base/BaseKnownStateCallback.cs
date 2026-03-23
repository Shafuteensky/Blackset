using System.Linq;
using Blackset.Duel.Participants;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Blackset.Inventories.Scripts.Items;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Коллбек от статуса раскрытия предмета
    /// </summary>
    public abstract class BaseKnownStateCallback : BaseVisualItemModule
    {
        protected DuelController duelController;
        
        protected string itemId;
        protected string ownerParticipantId;
        
        protected ItemClass itemClass;

        protected virtual void OnDestroy()
        {
            duelController?.EventHub.Unsubscribe<ConsumableSelectionCompletedEvent>(OnConsumableDeclared);
            duelController?.EventHub.Unsubscribe<DiceDeclarationCompletedEvent>(OnDiceDeclared);
            duelController?.EventHub.Unsubscribe<DiceSelectionCompletedEvent>(OnDiceDeclared);
        }

        public override void Initialize(VisualItemContext context)
        {
            itemClass = context.ItemClass;
            
            itemId = context.ItemId;
            ownerParticipantId = context.OwnerParticipantId;
            duelController = context.DuelController;

            if (duelController.DuelContext.Participants.TryGetValue(ownerParticipantId, out var participant))
            {
                switch (itemClass)
                {
                    case ItemClass.Consumable:
                        duelController.EventHub.Subscribe<ConsumableSelectionCompletedEvent>(OnConsumableDeclared);
                        break;
                    case ItemClass.Dice:
                        duelController.EventHub.Subscribe<DiceDeclarationCompletedEvent>(OnDiceDeclared);
                        duelController.EventHub.Subscribe<DiceSelectionCompletedEvent>(OnDiceDeclared);
                        break;
                }
            }
        }

        protected bool IsItemRevealed()
        {
            KnowledgeState participantKnowledge = duelController.DuelContext.Knowledge[ownerParticipantId];
            bool itemRevealed = false;
            
            switch (itemClass)
            {
                case ItemClass.Consumable:
                    itemRevealed = participantKnowledge.RevealedConsumables.Contains(itemId);
                    break;
                case ItemClass.Dice:
                    itemRevealed = participantKnowledge.RevealedDices.Contains(itemId);
                    break;
            }

            return itemRevealed;
        }

        private void OnConsumableDeclared(ConsumableSelectionCompletedEvent handler)
        {
            if (handler.ConsumableId != itemId) return;
            OnStateUpdate();
        }
        
        private void OnDiceDeclared(DiceDeclarationCompletedEvent handler)
        {
            if (handler.DiceId != itemId) return;
            OnStateUpdate();
        }
        
        private void OnDiceDeclared(DiceSelectionCompletedEvent handler)
        {
            if (handler.DiceId != itemId) return;
            OnStateUpdate();
        }

        protected abstract void OnStateUpdate();
    }
}