using System;
using System.Collections;
using Extensions.Coroutines;
using Extensions.Data.InMemoryData;
using Extensions.Data.InMemoryData.SelectionContext;
using Extensions.ScriptableValues;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackset.UI.HoverInfo
{
    /// <summary>
    /// Абстракция эмиттера запроса вывода информации по наведению курсора на объект
    /// </summary>
    public abstract class BaseHoverInfoEmitter<TContainer, TEntry, TElement> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        where TContainer : InMemoryDataContainer<TEntry> 
        where TEntry : InMemoryDataEntry
        where TElement : ContextIdHolder<TContainer, TEntry>
    {
        #region События

        /// <summary>
        /// Запрос вывода информации
        /// </summary>
        /// <typeparam name="TInventory">Ивентарь, запрашиваемый для вывода информации</typeparam>
        /// <typeparam name="string">Идентификатор ячейки инвентаря</typeparam>
        /// <typeparam name="Vector2">Позиция UI-элемента</typeparam>
        public static event Action<TContainer, string, Vector2> onShowRequested;
        /// <summary>
        /// Запрос прекращения вывода информации
        /// </summary>
        public static event Action onHideRequested;

        #endregion
        
        [Header("Параметры ховера"), Space]
        [SerializeField]
        [Tooltip("UI-элемент, откуда передаются данные")]
        private TElement dtatSourceElement;
        [SerializeField]
        [Tooltip("Время удержания курсора над элементом до отправки запроса на вывод информации")]
        private FloatValue holdSeconds;

        private bool isPointerInside;
        private CoroutineTask holdTask;

        private void Awake() => holdTask = new CoroutineTask(this);
        private void OnDisable() => HideInfo();
        
        
        public void OnPointerEnter(PointerEventData eventData) => ScheduleShow();
        public void OnPointerExit(PointerEventData eventData) => HideInfo();

        private void HideInfo()
        {
            isPointerInside = false;
            holdTask.Stop();
            if (dtatSourceElement != null) onHideRequested?.Invoke();
        }

        private void ScheduleShow()
        {
            isPointerInside = true;
            if (dtatSourceElement == null) return;
            holdTask.Start(HoldRoutine());
        }

        private IEnumerator HoldRoutine()
        {
            float time = 0f;

            while (time < holdSeconds.Value)
            {
                if ( !isPointerInside || 
                     !isActiveAndEnabled ) 
                    yield break;

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            if ( !isPointerInside || 
                 !isActiveAndEnabled || 
                 dtatSourceElement == null ) 
                yield break;

            onShowRequested?.Invoke(dtatSourceElement.DataContainer, dtatSourceElement.EntryId, dtatSourceElement.transform.position);
        }
    }
}