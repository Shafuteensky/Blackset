using Blackset.Data.Items.Types;
using Blackset.Effects;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Абстракция контроллера выводимой инфомрации о предмете инвентаря
    /// </summary>
    public abstract class GenericInventoryItemInfoPopupController<TInventory, TItemCell, TData, TItemType> : MonoBehaviour
        where TInventory : BaseInventory<TItemCell, TData, TItemType>
        where TItemCell : BaseItemCell<TData, TItemType>
        where TData : EffectingItemData
        where TItemType : BaseItemType
    {
        [Header("Ховер-панель"), Space]
        [SerializeField]
        protected CanvasGroup canvasGroup;
        [SerializeField]
        protected RectTransform popupRect;
        [SerializeField]
        protected GameObject budgetIndicator;
        [SerializeField]
        protected Vector2 screenOffset = new Vector2(16f, -16f);
        
        [Header("Графика"), Space]
        [SerializeField]
        protected Image iconImage;
        [SerializeField]
        protected Image rarityImage;

        [Header("Текст"), Space]
        [SerializeField]
        protected TMP_Text titleText;
        [SerializeField]
        protected TMP_Text typeText;
        [SerializeField]
        protected TMP_Text setText;
        [SerializeField]
        protected TMP_Text descriptionText;
        [SerializeField]
        protected TMP_Text priceText;
        [SerializeField]
        protected TMP_Text budgetText;

        protected Vector2 screenPosition;

        protected virtual void Awake()
        {
            if (canvasGroup == null) return;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        
        protected virtual void OnEnable()
        {
            GenericInventoryItemHoverInfoEmitter<TInventory, TItemCell, TData, TItemType>.onShowRequested += OnShowRequested;
            GenericInventoryItemHoverInfoEmitter<TInventory, TItemCell, TData, TItemType>.onHideRequested += OnHideRequested;
            Reset();
            HideImmediate();
        }

        protected virtual void OnDisable()
        {
            GenericInventoryItemHoverInfoEmitter<TInventory, TItemCell, TData, TItemType>.onShowRequested -= OnShowRequested;
            GenericInventoryItemHoverInfoEmitter<TInventory, TItemCell, TData, TItemType>.onHideRequested -= OnHideRequested;
        }

        protected void OnShowRequested(TInventory inventory, string cellId, Vector2 position)
        {
            Reset();
            
            screenPosition = position;
            if ( inventory == null || string.IsNullOrEmpty(cellId) ) return;
            var cell = inventory.GetById(cellId);
            if (cell == null || cell.IsEmpty) return;

            var item = cell.GetItemData(inventory.DataRegistry);
            var type = cell.GetTypeData(inventory.TypeRegistry);
            if (type == null) return;

            if (titleText != null) titleText.text = item.DataName;
            if (typeText != null) typeText.text = type.DataName;
            if (setText != null) setText.text = item.Set.DataName;
            if (descriptionText != null) descriptionText.text = item.DataDescription;
            if (priceText != null) priceText.text = item.GetPrice(type).ToString();
            if (iconImage != null)
            {
                iconImage.sprite = type.Icon;
                iconImage.color = item.Color;
                iconImage.enabled = type.Icon != null;

                if (item != null)
                {
                    iconImage.color = item.Color;
                    if (cell.IsDefault)
                    {
                        var c = iconImage.color;
                        c.a = 0.5f;
                        iconImage.color = c;
                    }
                }
            }
            
            budgetIndicator.SetActive(budgetText != null);
            OnDataShow(item);
            
            UpdateHoverPanelPosition();
            ShowImmediate();
        }

        protected void OnHideRequested() => HideImmediate();

        protected void ShowImmediate()
        {
            if (canvasGroup == null) return;
            popupRect.gameObject.SetActive(true);
        }

        protected void HideImmediate()
        {
            if (canvasGroup == null) return;
            popupRect.gameObject.SetActive(false);
        }

        protected void Reset()
        {
            budgetIndicator.SetActive(false);
        }

        protected virtual void OnDataShow(TData data) { }

        #region Hover Panel Position
        
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
