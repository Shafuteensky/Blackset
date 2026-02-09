using Extensions.Log;
using UnityEngine;
using UnityEngine.Audio;

namespace Extensions.Audio
{
    /// <summary>
    /// Базовый плеер аудио
    /// </summary>
    public abstract class BaseAudioPlayer : MonoBehaviour
    {
        [Header("Воспроизведение")]

        [SerializeField]
        [Tooltip("Тип аудио трека")]
        protected AudioModel model = AudioModel.Sfx;
        [SerializeField]
        [Tooltip("Оставить пустым, если нужны дефолтные значения от типа аудио")]
        protected AudioSpatialPreset spatialPreset;
        
        protected AudioController audioController;

        protected virtual void OnEnable()
        {
            audioController = AudioController.Instance;

            if (audioController == null)
            {
                ServiceDebug.LogError("Контроллер аудио не найден, звуки воспроизводиться не будут");
            }
        }

        protected virtual void Play(AudioResource audioResource)
        {
            if (audioController == null) return;
            
            Vector3 position = Vector3.zero;
            if (spatialPreset.SpatialBlend != 0) position = this.transform.position;
            audioController.Play(audioResource, position, model, spatialPreset);
        }
    }
}