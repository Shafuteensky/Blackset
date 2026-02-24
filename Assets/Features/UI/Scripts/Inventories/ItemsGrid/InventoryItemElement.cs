using System;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Effects;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Blackset.Inventories.Items;
using Blackset.UI.HoverInfo;
using Extensions.Log;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blackset.UI.InventoryManagement
{
    /// <summary>
    /// Элемент UI фабрики содержимого инвентаря
    /// </summary>
    public sealed class InventoryItemElement : BaseContainerEntryElement<Inventory, InventoryCell>, 
        IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler
    {
        /// <summary>
        /// Canvas-группа
        /// </summary>
        public CanvasGroup CanvasGroup => canvasGroup;
        
        [Header("Графика"), Space]
        [SerializeField]
        private Image itemIconImage;
        [SerializeField]
        private Image setImage;
            
        [Header("Текст"), Space]
        [SerializeField]
        private TMP_Text amountText;
        [SerializeField]
        private TMP_Text setNameText;
        [SerializeField]
        private TMP_Text priceText;
        [SerializeField]
        private TMP_Text budgetText;
        
        [Header("Параметры Drag&Drop"), Space]
        [SerializeField]
        private CanvasGroup canvasGroup;
        [SerializeField]
        private bool canDrag = true;
        [SerializeField]
        private bool canDrop = true;

        private InventoryDragDropCoordinator dropCoordinator;
        
        private void OnEnable()
        {
            dropCoordinator = InventoryDragDropCoordinator.Instance;
            Initialize(dropCoordinator != null);
        }
        
        #region Drag'n'Drop

        public void OnBeginDrag(PointerEventData eventData)
        {
            if ( !IsInitialized || !canDrag ) return;
            itemIconImage.CrossFadeAlpha(0.25f, 0.1f, false);
            dropCoordinator.BeginDrag(dataContainer, itemCellId);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if ( !IsInitialized || !canDrag ) return;
            dropCoordinator.UpdatePosition(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if ( !IsInitialized || !canDrag ) return;
            itemIconImage.CrossFadeAlpha(1, 0.1f, false);
            dropCoordinator.EndDrag();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if ( !IsInitialized || !canDrop ) return;

            dropCoordinator.RequestDrop(dataContainer, itemCellId);
        }

        #endregion

        /// <summary>
        /// Инициализация элемента
        /// </summary>
        /// <param name="newItemCellId">Идентификатор хранимых данных</param>
        public override void InitializeElement(Inventory newContainer, string newItemCellId = null)
        {
            base.InitializeElement(newContainer, newItemCellId);
            
            bool isNoCell = String.IsNullOrEmpty(newItemCellId);
            
            // Дроп-зона, не конкретная ячейка
            if (isNoCell)
            {
                canDrag = false;
                canDrop = true;
            }
            // Конкретная ячейка
            else
            {
                InventoryCell cell = newContainer.GetById(newItemCellId);
                
                if (cell == null)
                {
                    ServiceDebug.LogError($"Ячейка с идентификатором «{newItemCellId}» не найдена, инициализация провалена");
                    return;
                }
                
                InventoryItem cellItem = newContainer.GetCellItemData(cell);
                InventoryItemType cellItemTypeData = newContainer.GetCellTypeData(cell);
                
                if (cellItemTypeData == null || cellItem == null)
                {
                    ServiceDebug.LogError("Данные ячейки не полные, инициализация провалена");
                    return;
                }
                
                SetDragDropConfig(cell);
                SetIcon(cell, cellItem, cellItemTypeData);
                
                SetAmountText(cell);
                SetItemPriceText(cellItem, cellItemTypeData);

                // Параметры для EffectingItem
                if (cellItem is not EffectingItem effectingItem) return;
                SetIconColor(cell, effectingItem);
                SetItemSetText(effectingItem);
                
                // Параметры для DiceItem
                if (cellItem is not DiceData diceItem) return;
                SetDiceBudgetText(diceItem);
            }
        }

        #region Internal
        
        private void SetDragDropConfig(InventoryCell cell)
        {
            if ( String.IsNullOrEmpty(itemCellId) || ( cell != null && (cell.IsDefault || cell.IsEmpty) ) )
            {
                canDrag = false;
                canDrop = true;
            }
        }

        private void SetIcon(InventoryCell cell, InventoryItem item, InventoryItemType itemType)
        {
            itemIconImage.sprite = itemType.Icon;
        }

        private void SetIconColor(InventoryCell cell, EffectingItem item)
        {
            if (cell.IsDefault)
            {
                Color newColor = item.Color;
                newColor.a = 0.5f;
                itemIconImage.color = newColor;
            }
            else
            {
                itemIconImage.color = item.Color;
                if (setImage != null) setImage.color = item.Set.Color;
            }
        }

        private void SetAmountText(InventoryCell cell)
        {
            if (amountText != null)
            {
                if ( cell is {ItemAmount: > 1} )
                    amountText.text = $"x{cell.ItemAmount}";
                else
                    amountText.text = String.Empty;
            }
        }

        private void SetItemSetText(EffectingItem cellItem)
        {
            if (setNameText != null) setNameText.text = cellItem.Set.DataName;
        }

        private void SetItemPriceText(InventoryItem cellItem, InventoryItemType cellItemTypeData)
        {
            if (priceText != null)
            {
                int cellItemPrice = cellItem.GetPrice(cellItemTypeData);
                priceText.text = cellItemPrice.ToString();
            }
        }

        private void SetDiceBudgetText(DiceData cellData)
        {
            if (budgetText == null) return;
            budgetText.text = cellData.BudgetPrice.ToString();
        }
        
        #endregion
    }
}