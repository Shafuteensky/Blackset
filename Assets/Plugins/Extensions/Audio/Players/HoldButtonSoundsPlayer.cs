using Extensions.Generics;
using Extensions.Log;
using UnityEngine;
using UnityEngine.Audio;

namespace Extensions.Audio
{
    /// <summary>
    /// Воспроизведение звуков кнопкой с удержанием
    /// </summary>
    public class HoldButtonSoundsPlayer : BaseAudioPlayer
    {
        [Header("Звуки"), Space]
        [SerializeField]
        protected AudioResource onHoldCompleteSound;
        // TODO добавить звук процесса зажатия
        
        [Header("Кнопка"), Space]
        [SerializeField]
        protected AbstractHoldButton holdButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (holdButton == null)
            {
                ServiceDebug.LogError("Кнопка не назначена");
                return;
            }
            holdButton.onHoldCompleted += PlayOnHoldCompleted;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (holdButton == null) return;
            holdButton.onHoldCompleted -= PlayOnHoldCompleted;
        }
        
        protected void PlayOnHoldCompleted()
        {
            if (onHoldCompleteSound == null) return;
            Play(onHoldCompleteSound);
        }
    }
}