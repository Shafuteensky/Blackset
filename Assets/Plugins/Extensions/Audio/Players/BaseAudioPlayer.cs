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
        [Header("Объемное аудио")]

        [SerializeField]
        protected bool is3D;
        [SerializeField]
        [Min(0f)]
        protected float minDistance = 1f;
        [SerializeField]
        [Min(0f)]
        protected float maxDistance = 15f;
        
        [Header("Воспроизведение")]

        [SerializeField]
        protected AudioModel model = AudioModel.Music;
        
        protected AudioController audioController;

        protected virtual void OnEnable()
        {
            audioController = AudioController.Instance;

            if (audioController == null)
            {
                ServiceDebug.LogError("Контроллер аудио не найден, звуки воспроизводиться не будут");
            }
        }

        protected virtual void Play(AudioResource audioResource, Vector3 position = new())
        {
            if (audioController == null) return;
            
            if (position == Vector3.zero) audioController.Play(audioResource, model);
            else audioController.Play(audioResource, position, model);
        }
    }
}