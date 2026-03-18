using System;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Effects;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Blackset.Inventories.Items;
using Extensions.Data.InMemoryData.SelectionContext;
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
    public sealed class InventoryItemElement : ContextIdHolder<Inventory, InventoryCell>, 
        IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler
    {
        /// <summary>
        /// Canvas-группа
        /// </summary>
        public CanvasGroup CanvasGroup => canvasGroup;
        
        public InventoryCell Cell { get; private set; }
        public InventoryItem CellItem { get; private set; }
        public InventoryItemType CellItemType { get; private set; }
        
        [Header("Модули"), Space]
        [SerializeField] private BaseInventoryItemModule[] modules;
        // TODO Передлать остальные элементы как модули

        [Header("Графика"), Space]
        [SerializeField] private Image itemIconImage;
        [SerializeField] private Image setImage;
        [SerializeField] private GameObject newItemIndicator;
            
        [Header("Текст"), Space]
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private TMP_Text setNameText;
        
        [Header("Бюджетная стоимость"), Space]
        [SerializeField] private TMP_Text budgetText;
        [SerializeField] private GameObject budgetIndicator;
        
        [Header("Параметры Drag&Drop"), Space]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private bool canDrag = true;
        [SerializeField] private bool canDrop = true;

        private InventoryDragDropCoordinator dropCoordinator;
        
        
        private void OnEnable()
        {
            dropCoordinator = InventoryDragDropCoordinator.Instance;
        }
        
        #region Drag'n'Drop

        public void OnBeginDrag(PointerEventData eventData)
        {
            if ( dropCoordinator == null || !canDrag ) return;
            itemIconImage.CrossFadeAlpha(0.25f, 0.1f, false);
            dropCoordinator.BeginDrag(dataContainer, EntryId);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if ( dropCoordinator == null || !canDrag ) return;
            dropCoordinator.UpdatePosition(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if ( dropCoordinator == null || !canDrag ) return;
            itemIconImage.CrossFadeAlpha(1, 0.1f, false);
            dropCoordinator.EndDrag();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if ( dropCoordinator == null || !canDrop ) return;

            dropCoordinator.RequestDrop(dataContainer, EntryId);
        }

        #endregion

        #region Pointer events

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(EntryId)) return;
            
            UpdateNewItemIndicator(DataContainer.GetById(EntryId), true);
        }

        #endregion
        
        /// <summary>
        /// Инициализация элемента
        /// </summary>
        /// <param name="newContainer">Хранилище данных</param>
        /// <param name="newItemCellId">Идентификатор хранимых данных</param>
        public override void Initialize(Inventory newContainer, string newItemCellId)
        {
            base.Initialize(newContainer, newItemCellId);
            
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
                Cell = newContainer.GetById(newItemCellId);
                
                if (Cell == null)
                {
                    ServiceDebug.LogError($"Ячейка с идентификатором «{newItemCellId}» не найдена, инициализация провалена");
                    return;
                }
                
                CellItem = newContainer.GetCellItemData(Cell);
                CellItemType = newContainer.GetCellTypeData(Cell);
                
                if (CellItemType == null || CellItem == null)
                {
                    ServiceDebug.LogError("Данные ячейки не полные, инициализация провалена");
                    return;
                }
                
                SetDragDropConfig(Cell);
                SetIcon(Cell, CellItem, CellItemType);
                
                SetAmountText(Cell);
                
                UpdateNewItemIndicator(Cell);

                // Инициализация модулей
                foreach (var module in modules)
                    module.Initialize(this, newContainer);
                
                // Параметры для EffectingItem
                if (CellItem is not EffectingItem effectingItem) return;
                SetIconColor(Cell, effectingItem);
                SetItemSetText(effectingItem);
                
                // Параметры для DiceItem
                if (CellItem is not DiceData diceItem) return;
                SetDiceBudgetText(diceItem);
            }
        }

        #region Internal
        
        private void SetDragDropConfig(InventoryCell cell)
        {
            if ( String.IsNullOrEmpty(EntryId) || ( cell != null && (cell.IsDefault || cell.IsEmpty) ) )
            {
                canDrag = false;
                canDrop = true;
            }
        }

        private void SetIcon(InventoryCell cell, InventoryItem item, InventoryItemType itemType)
        {
            if (itemIconImage != null) 
                itemIconImage.sprite = itemType.Icon;
        }

        private void SetIconColor(InventoryCell cell, EffectingItem item)
        {
            if (itemIconImage == null) return;
            
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

        private void SetDiceBudgetText(DiceData cellData)
        {
            if (budgetIndicator == null) return;
            budgetIndicator.SetActive(cellData.BudgetPrice > 0);
            
            if (budgetText == null) return;
            budgetText.text = cellData.BudgetPrice.ToString();
        }

        private void UpdateNewItemIndicator(InventoryCell cell, bool markSeen = false)
        {
            if (newItemIndicator == null) return;

            if (cell == null) return;
            
            if (markSeen) cell.MarkSeen();
            newItemIndicator.SetActive(cell.IsNew);
        }
        
        #endregion
    }
}