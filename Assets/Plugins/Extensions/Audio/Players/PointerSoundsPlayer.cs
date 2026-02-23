using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

namespace Extensions.Audio
{
    /// <summary>
    /// Воспроизведение звуков кнопкой
    /// </summary>
    public class PointerSoundsPlayer : BaseAudioPlayer, IPointerEnterHandler, IPointerDownHandler
    {
        [Header("Звуки"), Space]
        [SerializeField]
        protected AudioResource enterSound;
        [SerializeField]
        protected AudioResource downSound;

        #region IPointerHandlers
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enterSound != null) Play(enterSound);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (downSound != null) Play(downSound);
        }
        
        #endregion
    }
}