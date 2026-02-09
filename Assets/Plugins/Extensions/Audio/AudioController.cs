using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using Extensions.Coroutines;
using Extensions.Log;
using Extensions.Pool;
using Extensions.Singleton;
using Extensions.Helpers;

namespace Extensions.Audio
{
    
    /// <summary>
    /// Контроллер аудио
    /// </summary>
    /// <remarks>
    /// Воспроизводит звуки, музыку и эмбиенс программы
    /// </remarks>
    public sealed class AudioController : MonoBehaviourSingleton<AudioController>
    {
        private const int PREWARM_SOURCES_NUMBER = 4;
        
        [Header("Значения по-умолчанию")]

        [SerializeField]
        private AudioDefaults musicDefaults;
        [SerializeField] private AudioDefaults ambienceDefaults;
        [SerializeField] private AudioDefaults uiDefaults;
        [SerializeField] private AudioDefaults sfxDefaults;

        [Header("Фабрика источников аудио")]

        [SerializeField]
        private AudioSource oneShotPrefab;
        [SerializeField] private Transform oneShotRoot;
        [SerializeField, Min(1)] private int oneShotMaxInstances = 32;

        private ObjectPool<AudioSource> oneShotPool;
        private readonly Dictionary<AudioSource, CoroutineTask> releaseTasks = new();

        #region MonoLifeCycle
        
        protected override void Awake()
        {
            base.Awake();

            if (oneShotPrefab == null)
            {
                return;
            }

            if (oneShotRoot == null)
            {
                oneShotRoot = transform;
            }

            oneShotPool = new ObjectPool<AudioSource>(
                oneShotPrefab,
                oneShotRoot,
                PREWARM_SOURCES_NUMBER,
                oneShotMaxInstances
            );
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<AudioSource, CoroutineTask> pair in releaseTasks)
            {
                if (pair.Value == null) continue;
                pair.Value.Stop();
            }

            releaseTasks.Clear();
        }

        #endregion
        
        /// <summary>
        /// Воспроизвести аудио (2D по-умолчанию)
        /// </summary>
        /// <param name="resource">Аудио-ресурс</param>
        public void Play(AudioResource resource, AudioModel model = AudioModel.UI)
        {
            Play(resource, null, null, model);
        }
        
        /// <summary>
        /// Воспроизвести аудио (в определенной точке, 3D по-умолчанию)
        /// </summary>
        /// <param name="resource">Аудио-ресурс</param>
        public void Play(AudioResource resource, Vector3 position, AudioModel model = AudioModel.Sfx)
        {
            Play(resource, position, null, model);
        }

        /// <summary>
        /// Воспроизвести аудио (с закреплением за объектом, 3D по-умолчанию)
        /// </summary>
        /// <param name="resource">Аудио-ресурс</param>
        public void Play(AudioResource resource, Transform followTarget, AudioModel model = AudioModel.Sfx)
        {
            Play(resource, null, followTarget, model);
        }
        
        #region Settings

        /// <summary>
        /// Получить дефолтные настройки в зависимости от типа аудио
        /// </summary>
        /// <param name="type">Тип аудио трека</param>
        /// <returns>Настройки аудио</returns>
        public AudioDefaults GetDefaults(AudioModel type)
        {
            if (type == AudioModel.Music) return musicDefaults;
            if (type == AudioModel.Ambience) return ambienceDefaults;
            if (type == AudioModel.UI) return uiDefaults;
            return sfxDefaults;
        }

        /// <summary>
        /// Построение настроек аудио
        /// </summary>
        /// <param name="defaults">Дефолтные параметры аудио трека</param>
        /// <param name="loop">Зацикленность</param>
        /// <returns></returns>
        public AppliedAudioSettings BuildSettings(AudioDefaults defaults, bool loop = false, AudioDefaults defaultsOverrides = new())
        {
            AppliedAudioSettings settings = new()
            {
                mixerGroup = defaults.mixerGroup,
                volume = defaults.volume
            };

            float minPitch = defaults.pitchMin;
            float maxPitch = defaults.pitchMax;
            if (maxPitch < minPitch)
            {
                (minPitch, maxPitch) = (maxPitch, minPitch);
            }

            settings.pitch = Random.Range(minPitch, maxPitch);

            settings.spatialBlend = defaults.spatialBlend;
            settings.minDistance = defaults.minDistance;
            settings.maxDistance = defaults.maxDistance;
            settings.priority = defaults.priority;
            settings.loop = loop;

            return settings;
        }

        /// <summary>
        /// Применить конфигурацию настроек аудио к источнику аудио
        /// </summary>
        /// <param name="source"></param>
        /// <param name="settings"></param>
        public void ApplySettings(AudioSource source, AppliedAudioSettings settings)
        {
            if (source == null)
            {
                ServiceDebug.LogError("Источник не задан, настройки аудио не применены");
                return;
            }

            source.outputAudioMixerGroup = settings.mixerGroup;

            source.volume = settings.volume;
            source.pitch = settings.pitch;

            source.spatialBlend = settings.spatialBlend;

            source.minDistance = settings.minDistance;
            source.maxDistance = settings.maxDistance;

            source.priority = settings.priority;

            source.loop = settings.loop;
        }
        
        #endregion
        
        #region Internal

        private void Play(AudioResource resource, Vector3? position, Transform followTarget, AudioModel model)
        {
            if (Logic.IsNull(resource, "Аудио-ресурс не назначен, аудио не воспроизведено")) return;
            if (!isPoolValid()) return;

            AudioSource source = oneShotPool.Get();
            if (source == null)
            {
                ServiceDebug.LogError("Ошибка создания источника звука, аудио не воспроизведено");
                return;
            }

            if (followTarget != null)
            {
                source.transform.position = followTarget.position;
            }
            else if (position.HasValue)
            {
                source.transform.position = position.Value;
            }

            AudioDefaults defaults = GetDefaults(model);
            AppliedAudioSettings settings = BuildSettings(defaults);
            ApplySettings(source, settings);

            source.resource = resource;
            source.Play();

            if (followTarget != null)
            {
                ReleaseAfterPlayFollow(source, followTarget);
                return;
            }

            ReleaseAfterPlay(source);
        }
        
        private void ReleaseAfterPlay(AudioSource source)
        {
            if (Logic.IsNull(source, "Ошибка источника звука, аудио не воспроизведено")) return;
            if (!isPoolValid()) return;

            GetReleaseTask(source).Start(ReleaseRoutine(source, null));
        }

        private void ReleaseAfterPlayFollow(AudioSource source, Transform followTarget)
        {
            if (Logic.IsNull(source, "Ошибка источника звука, аудио не воспроизведено")) return;
            if (!isPoolValid()) return;

            GetReleaseTask(source).Start(ReleaseRoutine(source, followTarget));
        }

        private CoroutineTask GetReleaseTask(AudioSource source)
        {
            if (releaseTasks.TryGetValue(source, out CoroutineTask task))
            {
                return task;
            }

            task = new CoroutineTask(this);
            releaseTasks.Add(source, task);
            return task;
        }

        private System.Collections.IEnumerator ReleaseRoutine(AudioSource source, Transform followTarget)
        {
            while (source.isPlaying)
            {
                if (followTarget != null)
                {
                    source.transform.position = followTarget.position;
                }

                yield return null;
            }

            source.Stop();
            source.resource = null;
            source.loop = false;

            oneShotPool.Release(source);

            if (releaseTasks.TryGetValue(source, out CoroutineTask task))
            {
                task.Stop();
            }
        }

        private bool isPoolValid()
        {
            if (oneShotPool == null)
            {
                ServiceDebug.LogError("Ошибка пула, аудио не воспроизведено");
                return false;
            }

            return true;
        }
        
        #endregion
    }
}
