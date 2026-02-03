using System;
using System.Collections;
using System.Collections.Generic;
using Extensions.Coroutines;
using Extensions.Log;
using Extensions.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Extensions.SceneFlow
{
    /// <summary>
    /// Контроллер сцен
    /// </summary>
    /// <remarks>
    /// Управляет переключением сцен и ответственен за переходы между ними
    /// </remarks>
    public class SceneController : MonoBehaviourSingleton<SceneController>
    {
        /// <summary>
        /// Пара (связка) сцена-идентификатор
        /// </summary>
        [System.Serializable]
        protected class SceneBinding
        {
            [field: SerializeField] public SceneID Id { get; private set; }

            [field: SerializeField] public string SceneName { get; private set; }
        }
        
        #region Events
        
        /// <summary>
        /// Событие начала загрузки сцены
        /// </summary>
        public event Action onLoadingStart;
        
        /// <summary>
        /// Событие обновления прогресса загрузки
        /// </summary>
        /// <returns>
        /// Процент загрузки от 0 до 1
        /// </returns>
        public event Action<float> onLoadingProgressUpdate;
        
        /// <summary>
        /// Событие окончания загрузки сцены
        /// </summary>
        public event Action onSceneLoaded;

        #endregion
        
        /// <summary>
        /// Состояние перехода между сценами
        /// </summary>
        /// <returns>true если в стадии перехода, false если переход завершен</returns>
        public bool IsTransitionInProgress => isTransitionInProgress;
        
        /// <summary>
        /// Текущий прогресс
        /// </summary>
        public float CurrentProgress => currentProgress;

        [Header("Стартовая сцена")]
        [SerializeField]
        protected SceneID firstScene = default;
        [SerializeField]
        protected bool loadFirstSceneOnStart = true;

        [Header("Сцены проекта")]
        [SerializeField]
        protected SceneID loadingScene = default;
        [SerializeField]
        protected List<SceneBinding> scenes = new List<SceneBinding>();

        protected bool isTransitionInProgress = false;
        protected float currentProgress = 0f;

        protected CoroutineTask transitionTask = default;
        [SerializeField]
        protected float loadingTimeout = 20f;
        [SerializeField]
        protected float targetTimeout = 60f;
        
        protected override void Awake()
        {
            base.Awake();
            transitionTask = new CoroutineTask(this);
        }

        protected virtual void Start()
        {
            if (!loadFirstSceneOnStart)
                return;

            if (firstScene == null)
            {
                ServiceDebug.LogError($"Не назначена стартовая сцена в {nameof(SceneController)}");
                return;
            }

            LoadSceneByID(firstScene.Id, false);
        }
        
        /// <summary>
        /// Загрузка сцены по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="additive">Аддитивно или с закрытием активных</param>
        public void LoadSceneByID(string id, bool additive = false)
        {
            if (isTransitionInProgress)
                return;

            if (string.IsNullOrEmpty(id))
            {
                ServiceDebug.LogError($"Пустой идентификатор при вызове {nameof(LoadSceneByID)}");
                return;
            }

            string sceneName = String.Empty;
            if (!TryGetSceneName(id, out sceneName))
            {
                ServiceDebug.LogError( $"Сцена с идентификатором «{id}» не найдена");
                return;
            }

            transitionTask.Start(TransitionRoutine(sceneName, additive));
        }

        protected bool TryGetSceneName(string id, out string sceneName)
        {
            sceneName = string.Empty;

            foreach (SceneBinding binding in scenes)
            {
                if (binding == null)
                    continue;

                if (binding.Id == null)
                    continue;

                if (binding.Id.Id == id)
                {
                    sceneName = binding.SceneName;
                    return !string.IsNullOrEmpty(sceneName);
                }
            }

            return false;
        }

        protected bool TryGetLoadingSceneName(out string sceneName)
        {
            sceneName = string.Empty;

            if (loadingScene == null)
                return false;

            return TryGetSceneName(loadingScene.Id, out sceneName);
        }

        protected IEnumerator WaitForAsyncOperation(
            AsyncOperation operation,
            float timeoutSeconds,
            string label,
            bool trackProgress)
        {
            float timer = 0f;

            while (!operation.isDone)
            {
                timer += Time.unscaledDeltaTime;

                if (trackProgress)
                {
                    currentProgress = operation.progress;
                    onLoadingProgressUpdate?.Invoke(currentProgress);
                }

                if (timer >= timeoutSeconds)
                {
                    ServiceDebug.LogError($"Таймаут загрузки: «{label}». progress={operation.progress}, " +
                                          $"allowSceneActivation={operation.allowSceneActivation}");

                    isTransitionInProgress = false;
                    FallbackToFirstScene();
                    yield break;
                }

                yield return null;
            }
        }

        protected IEnumerator TransitionRoutine(string targetSceneName, bool additive)
        {
            isTransitionInProgress = true;
            currentProgress = 0f;
            onLoadingStart?.Invoke();

            string loadingSceneName;
            if (!TryGetLoadingSceneName(out loadingSceneName))
            {
                ServiceDebug.LogError($"Не найдена loading-сцена в {nameof(SceneController)}");
                isTransitionInProgress = false;
                FallbackToFirstScene();
                yield break;
            }

            AsyncOperation loadLoading = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Single);
            if (loadLoading == null)
            {
                ServiceDebug.LogError($"Не удалось начать загрузку loading-сцены «{loadingSceneName}»");
                isTransitionInProgress = false;
                FallbackToFirstScene();
                yield break;
            }

            yield return WaitForAsyncOperation(loadLoading, loadingTimeout, loadingSceneName, false);
            if (!isTransitionInProgress)
                yield break;

            LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;

            AsyncOperation loadTarget = SceneManager.LoadSceneAsync(targetSceneName, mode);
            if (loadTarget == null)
            {
                ServiceDebug.LogError($"Не удалось начать загрузку целевой сцены «{targetSceneName}»");
                isTransitionInProgress = false;
                FallbackToFirstScene();
                yield break;
            }

            yield return WaitForAsyncOperation(loadTarget, targetTimeout, targetSceneName, true);
            if (!isTransitionInProgress)
                yield break;

            if (additive)
            {
                Scene loadedScene = SceneManager.GetSceneByName(targetSceneName);
                if (loadedScene.IsValid())
                    SceneManager.SetActiveScene(loadedScene);
            }

            currentProgress = 1f;
            isTransitionInProgress = false;
            onSceneLoaded?.Invoke();
        }
        
        protected void FallbackToFirstScene()
        {
            if (firstScene == null)
            {
                ServiceDebug.LogError($"Фолбэк невозможен: не назначена стартовая сцена");
                return;
            }

            string fallbackSceneName;
            if (!TryGetSceneName(firstScene.Id, out fallbackSceneName))
            {
                ServiceDebug.LogError($"Фолбэк невозможен: стартовая сцена не найдена в списке сцен");
                return;
            }

            ServiceDebug.LogWarning($"Переход в фолбэк-сцену «{fallbackSceneName}»");
            transitionTask.Start(TransitionRoutine(fallbackSceneName, false));
        }

    }
}
