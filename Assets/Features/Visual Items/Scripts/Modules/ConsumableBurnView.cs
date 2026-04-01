using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Extensions.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Визульная репрезентация расходника
    /// </summary>
    public class ConsumableBurnView : BaseVisualItemModule
    {
        [Header("Звук уничтожения расходника"), Space]
        [SerializeField] private AudioResource burnSound;

        private string itemId;
        private string ownerParticipantId;
        
        private DuelController duelController;
        protected AudioController audioController;
        
        public override void Initialize(VisualItemContext context)
        {
            itemId = context.ItemId;
            ownerParticipantId = context.OwnerParticipantId;
            
            duelController = context.DuelController;
            audioController = AudioController.Instance;
            
            duelController.EventHub.Subscribe<ConsumableBurnedEvent>(BurnConsumable);
        }
        
        private void OnDestroy()
        {
            duelController?.EventHub.Unsubscribe<ConsumableBurnedEvent>(BurnConsumable);
        }

        private void BurnConsumable(ConsumableBurnedEvent handler)
        {
            // Только если этот расходник принадлежит бросившему, и брошен был именно этот расходник
            if (handler.ParticipantId != ownerParticipantId || handler.ChosenConsumableId != itemId) return;
            
            audioController.Play(burnSound, transform.position);
            gameObject.SetActive(false); // TODO Заменить на эффект сгорания модели
        }
    }
}