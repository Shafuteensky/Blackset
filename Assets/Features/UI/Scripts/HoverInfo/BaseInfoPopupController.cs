using Extensions.Data.InMemoryData;
using UnityEngine;

namespace Blackset.UI.HoverInfo
{
    /// <summary>
    /// Абстракция контроллера вывода информации по запросу
    /// </summary>
    public abstract class BaseInfoPopupController<TContainer, TEntry, TElement> : MonoBehaviour
        where TContainer : InMemoryDataContainer<TEntry> 
        where TEntry : InMemoryDataEntry
        where TElement : BaseContainerEntryElement<TContainer, TEntry>
    {
        [Header("Ховер-панель"), Space]
        [SerializeField]
        protected CanvasGroup canvasGroup;
        [SerializeField]
        protected RectTransform popupRect;
        [SerializeField]
        protected Vector2 screenOffset = new Vector2(16f, -16f);
        
        protected Vector2 screenPosition;

        protected virtual void Awake()
        {
            if (canvasGroup == null) return;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        
        protected virtual void OnEnable()
        {
            BaseHoverInfoEmitter<TContainer, TEntry, TElement>.onShowRequested += OnShowRequested;
            BaseHoverInfoEmitter<TContainer, TEntry, TElement>.onHideRequested += OnHideRequested;
            OnResetElements();
            OnHideRequested();
        }

        protected virtual void OnDisable()
        {
            BaseHoverInfoEmitter<TContainer, TEntry, TElement>.onShowRequested -= OnShowRequested;
            BaseHoverInfoEmitter<TContainer, TEntry, TElement>.onHideRequested -= OnHideRequested;
        }

        #region Показ ховер-панели
        
        protected void OnShowRequested(TContainer inventory, string cellId, Vector2 position)
        {
            screenPosition = position;
            
            OnResetElements();
            OnShow(inventory, cellId, position);
            UpdateHoverPanelPosition();
            
            if (canvasGroup == null) return;
            popupRect.gameObject.SetActive(true);
        }
        
        protected void OnHideRequested()
        {
            OnHide();
            
            if (canvasGroup == null) return;
            popupRect.gameObject.SetActive(false);
        }

        protected virtual void OnShow(TContainer inventory, string cellId, Vector2 position) { }
        protected virtual void OnHide() { }
        protected virtual void OnResetElements() { }

        #endregion
        
        #region Положение ховер-панели
        
        protected void UpdateHoverPanelPosition()
        {
            if (popupRect == null) return;

            UpdateHoverPanelPivot(screenPosition);
            UpdateHoverPanelScreenPosition(screenPosition);
        }

        protected void UpdateHoverPanelPivot(Vector2 screenPos)
        {
            if (popupRect == null) return;

            Vector2 size = GetPopupSizeInScreenPixels();
            float w = size.x;
            float h = size.y;

            float freeRight = Screen.width - screenPos.x;
            float freeBottom = screenPos.y;

            float pivotX = freeRight >= w ? 0f : 1f;
            float pivotY = freeBottom >= h ? 1f : 0f;

            popupRect.pivot = new Vector2(pivotX, pivotY);
        }

        protected void UpdateHoverPanelScreenPosition(Vector2 screenPos)
        {
            if (popupRect == null) return;

            Vector2 pivot = popupRect.pivot;
            Vector2 offset = GetOffsetForPivot(pivot);

            popupRect.position = screenPos + offset;
        }

        protected Vector2 GetOffsetForPivot(Vector2 pivot)
        {
            float ox = pivot.x < 0.5f ? screenOffset.x : -screenOffset.x;
            float oy = pivot.y > 0.5f ? screenOffset.y : -screenOffset.y;

            return new Vector2(ox, oy);
        }

        protected Vector2 GetPopupSizeInScreenPixels()
        {
            if (popupRect == null) return Vector2.zero;

            Vector2 size = popupRect.rect.size;

            Canvas canvas = popupRect.GetComponentInParent<Canvas>();
            if (canvas == null) return size;

            return size * canvas.scaleFactor;
        }
        
        #endregion
    }
}