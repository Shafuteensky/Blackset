using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Extensions.Audio;
using Extensions.Log;
using UnityEngine;
using UnityEngine.Audio;

namespace Blackset.Data.Items.Visual.Sounds
{
    /// <summary>
    /// Воспроизводит случайный звук броска дайса по событию DiceRolledEvent.
    /// </summary>
    public sealed class DiceRollAudioPlayer : BaseAudioPlayer
    {
        [Header("Звуки броска"), Space]
        [SerializeField] private AudioResource rollSounds;

        private DuelController duelController;

        private void Awake() => duelController = DuelController.Instance;


        protected override void OnEnable()
        {
            base.OnEnable();
            duelController.EventHub.Subscribe<DiceRolledEvent>(OnDiceRolled);
        }
        
        private void OnDestroy()
        {
            if (duelController == null) return;
            
            duelController.EventHub.Unsubscribe<DiceRolledEvent>(OnDiceRolled);
        }

        private void OnDiceRolled(DiceRolledEvent handler)
        {
            if (rollSounds == null)
            {
                ServiceDebug.LogError($"[{nameof(DiceRollAudioPlayer)}] Не назначен контейнер звуков броска");
                return;
            }

            Play(rollSounds);
        }
    }
}