using Blackset.Inventories;
using Extensions.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace Features.DiceBudget
{
    /// <summary>
    /// Воспроизведение звука при недостатке бюджета пула дайсов
    /// </summary>
    public class OnBudgetLackSound : BaseAudioPlayer
    {
        [Header("Звуки"), Space]
        [SerializeField]
        protected AudioResource sound;

        protected override void OnEnable()
        {
            base.OnEnable();
            DicePoolInventory.onBudgetCheckFailed += PlaySound;
        }
        
        private void OnDisable()
        {
            DicePoolInventory.onBudgetCheckFailed -= PlaySound;
        }
        
        private void PlaySound(int _)
        {
            if (sound != null) Play(sound);
        }
    }
}