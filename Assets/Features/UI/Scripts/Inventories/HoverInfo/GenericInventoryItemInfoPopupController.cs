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
        [Header("Параметры элемента"), Space]
        [SerializeField]
        protected CanvasGroup canvasGroup;
        [SerializeField]
        protected RectTransform popupRect;
        [SerializeField]
        protected Vector2 screenOffset = new Vector2(16f, -16f);
        
        [Header("Графика"), Space]
        [SerializeField]
        protected Image iconImage;

        [Header("Текст"), Space]
        [SerializeField]
        protected TMP_Text titleText;
        [SerializeField]
        protected TMP_Text typeText;
        [SerializeField]
        protected TMP_Text descriptionText;
        [SerializeField]
        protected TMP_Text metaText;

        protected Vector2 screenPosition;

        protected virtual void OnEnable()
        {
            GenericInventoryItemHoverInfoEmitter<TInventory, TItemCell, TData, TItemType>.onShowRequested += OnShowRequested;
            GenericInventoryItemHoverInfoEmitter<TInventory, TItemCell, TData, TItemType>.onHideRequested += OnHideRequested;
            HideImmediate();
        }

        protected virtual void OnDisable()
        {
            GenericInventoryItemHoverInfoEmitter<TInventory, TItemCell, TData, TItemType>.onShowRequested -= OnShowRequested;
            GenericInventoryItemHoverInfoEmitter<TInventory, TItemCell, TData, TItemType>.onHideRequested -= OnHideRequested;
        }

        protected virtual void LateUpdate()
        {
            var pos = RectTransformUtility.WorldToScreenPoint(null, screenPosition);
            if (popupRect != null) popupRect.position = pos + screenOffset;
        }

        protected void OnShowRequested(TInventory inventory, string cellId, Vector2 position)
        {
            screenPosition = position;
            if ( inventory == null || string.IsNullOrEmpty(cellId) ) return;
            var cell = inventory.GetById(cellId);
            if (cell == null || cell.IsEmpty) return;

            var item = cell.GetItemData(inventory.DataRegistry);
            var type = cell.GetTypeData(inventory.TypeRegistry);
            if (type == null) return;

            if (titleText != null) titleText.text = item.name;
            if (typeText != null) typeText.text = type.name;

            if (descriptionText != null)
            {
                descriptionText.text = string.Empty;
            }

            if (metaText != null)
            {
                if (cell.ItemAmount > 1) metaText.text = $"x{cell.ItemAmount}";
                else metaText.text = string.Empty;
            }

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

            ShowImmediate();
        }

        protected void OnHideRequested() => HideImmediate();

        protected void ShowImmediate()
        {
            if (canvasGroup == null) return;

            popupRect.gameObject.SetActive(true);
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        protected void HideImmediate()
        {
            if (canvasGroup == null) return;

            popupRect.gameObject.SetActive(false);
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}
