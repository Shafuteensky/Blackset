using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Effects;
using Blackset.Inventory.Cells;
using Features.Inventory.Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Абстракция контроллера выводимой инфомрации о предмете инвентаря
    /// </summary>
    public class InventoryItemInfoPopupController : MonoBehaviour
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
            InventoryCellHoverInfoEmitter.onShowRequested += OnShowRequested;
            InventoryCellHoverInfoEmitter.onHideRequested += OnHideRequested;
            Reset();
            HideImmediate();
        }

        protected virtual void OnDisable()
        {
            InventoryCellHoverInfoEmitter.onShowRequested -= OnShowRequested;
            InventoryCellHoverInfoEmitter.onHideRequested -= OnHideRequested;
        }

        protected void OnShowRequested(Blackset.Inventory.Inventories.Inventory inventory, string cellId, Vector2 position)
        {
            Reset();
            
            screenPosition = position;
            if ( inventory == null || string.IsNullOrEmpty(cellId) ) return;
            InventoryCell cell = inventory.GetById(cellId);
            if (cell == null || cell.IsEmpty) return;

            InventoryItem item = cell.GetItemData(inventory.DataRegistry);
            InventoryItemType type = cell.GetTypeData(inventory.TypeRegistry);
            if (type == null) return;

            FillBaseInfo(item, type);
            FillPriceLabel(item, type);
            SetIcon(cell, item, type);
            
            if (item is EffectingItem effectingItem)
            {
                FillEffectingItemInfo(effectingItem);
            }
            
            UpdateHoverPanelPosition();
            ShowImmediate();
        }

        #region Элементы ховер-панели

        protected void Reset()
        {
            budgetIndicator.SetActive(false);
        }

        protected void FillBaseInfo(InventoryItem item, InventoryItemType type)
        {
            if (titleText != null) titleText.text = item.DataName;
            if (typeText != null) typeText.text = type.DataName;
            if (descriptionText != null) descriptionText.text = item.DataDescription;
        }

        protected void FillEffectingItemInfo(EffectingItem effectingItem)
        {
            if (setText != null) setText.text = effectingItem.Set.DataName;
            if (iconImage != null) iconImage.color = effectingItem.Color;

            if (effectingItem is DiceItem diceItem)
            {
                budgetIndicator.SetActive(true);
                if (budgetText != null) budgetText.text = diceItem.BudgetPrice.ToString();
            }
        }

        protected void FillPriceLabel(InventoryItem item, InventoryItemType type)
        {
            if (priceText != null) priceText.text = item.GetPrice(type).ToString();
        }

        protected void SetIcon(InventoryCell cell, InventoryItem item, InventoryItemType type)
        {
            if (iconImage == null) return;
            
            iconImage.sprite = type.Icon;
            iconImage.enabled = type.Icon != null;

            if (cell.IsDefault)
            {
                var c = iconImage.color;
                c.a = 0.5f;
                iconImage.color = c;
            }
        }
        
        #endregion

        #region Показ ховер-панели
        
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
