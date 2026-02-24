using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Effects;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Blackset.Inventories.Items;
using Blackset.UI.HoverInfo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blackset.UI.InventoryManagement
{
    /// <summary>
    /// Контроллер выводимой информации о предмете инвентаря
    /// </summary>
    public class InventoryItemInfoPopupController : BaseInfoPopupController<Inventory, InventoryCell, InventoryItemElement>
    {
        [SerializeField]
        protected GameObject budgetIndicator;
        
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
        
        protected override void OnShow(Inventory inventory, string cellId, Vector2 position)
        {
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
        }

        #region Элементы ховер-панели

        protected override void OnResetElements()
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

            if (effectingItem is DiceData diceItem)
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
    }
}
