using System;
using System.Collections;
using Extensions.Coroutines;
using Extensions.Data.InMemoryData;
using Extensions.Data.InMemoryData.SelectionContext;
using Extensions.ScriptableValues;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Blackset.UI.HoverInfo
{
    /// <summary>
    /// Абстракция эмиттера запроса вывода информации по наведению курсора на объект
    /// </summary>
    public abstract class BaseHoverInfoEmitter<TContainer, TEntry, TElement> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
        where TContainer : InMemoryDataContainer<TEntry> 
        where TEntry : InMemoryDataEntry
        where TElement : ContextIdHolder<TContainer, TEntry>
    {
        #region События

        /// <summary>
        /// Запрос вывода информации
        /// </summary>
        public static event Action<TContainer, string, Vector2> onShowRequested;
        /// <summary>
        /// Запрос прекращения вывода информации
        /// </summary>
        public static event Action onHideRequested;
        /// <summary>
        /// Обновление положения ховер-панели (пока курсор внутри объекта)
        /// </summary>
        public static event Action<Vector2> onPositionUpdated;

        #endregion
        
        [Header("Параметры ховера"), Space]
        [SerializeField]
        [Tooltip("UI-элемент, откуда передаются данные")]
        private TElement dtatSourceElement;
        [SerializeField]
        [Tooltip("Время удержания курсора над элементом до отправки запроса на вывод информации")]
        private FloatValue holdSeconds;
        [SerializeField]
        [Tooltip("Откуда брать точку вывода ховер-панели")]
        private PopupPositionSource positionSource = PopupPositionSource.Pointer;

        private bool isPointerInside;
        private bool isPopupVisible;
        private CoroutineTask holdTask;

        private void Awake() => holdTask = new CoroutineTask(this);
        private void OnDisable() => HideInfo();

        public void OnPointerEnter(PointerEventData eventData) => ScheduleShow();
        public void OnPointerExit(PointerEventData eventData) => HideInfo();
        public void OnPointerMove(PointerEventData eventData)
        {
            // Обновляем позицию только если панель уже показана и режим — по курсору
            if (!isPopupVisible || positionSource != PopupPositionSource.Pointer) return;
            onPositionUpdated?.Invoke(eventData.position);
        }

        private void HideInfo()
        {
            isPointerInside = false;
            isPopupVisible = false;
            holdTask.Stop();
            if (dtatSourceElement != null) onHideRequested?.Invoke();
        }

        private void ScheduleShow()
        {
            isPointerInside = true;
            isPopupVisible = false;
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
            
            // Точка вывода панели
            Vector2 popupPos = positionSource switch
            {
                PopupPositionSource.Pointer => Mouse.current != null
                    ? Mouse.current.position.ReadValue()
                    : Touchscreen.current.primaryTouch.position.ReadValue(),
                    _ => GetScreenPosition()
            };
            
            isPopupVisible = true;
            
            // Запрос на вывод в нужной точке
            onShowRequested?.Invoke(
                dtatSourceElement.DataContainer, 
                dtatSourceElement.EntryId, 
                popupPos);
        }
        
        private Vector2 GetScreenPosition()
        {
            if (dtatSourceElement.transform is RectTransform rectTransform)
            {
                Canvas canvas = dtatSourceElement.GetComponentInParent<Canvas>();
                Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                    ? canvas.worldCamera
                    : null;
                return RectTransformUtility.WorldToScreenPoint(cam, rectTransform.position);
            }

            if (Camera.main != null) return Camera.main.WorldToScreenPoint(dtatSourceElement.transform.position);
            else return Vector2.zero;
        }
    }
}