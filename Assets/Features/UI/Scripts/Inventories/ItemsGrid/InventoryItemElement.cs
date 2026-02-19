using System;
using Blackset.Data.Items.Types;
using Blackset.Effects;
using Blackset.Inventory.Cells;
using Extensions.Generics;
using Extensions.Log;
using Features.Inventory.Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Элемент UI фабрики содержимого инвентаря
    /// </summary>
    public class InventoryItemElement : InitializableMonoBehaviour, 
        IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler
    {
        /// <summary>
        /// Canvas-группа
        /// </summary>
        public CanvasGroup CanvasGroup => canvasGroup;
        
        /// <summary>
        /// Прилинкованный инвентарь элемента
        /// </summary>
        public Blackset.Inventory.Inventories.Inventory Inventory => inventory;
        /// <summary>
        /// Идентификатор прилинкованной ячейки инвентаря
        /// </summary>
        public string ItemCellId => itemCellId;
        
        [SerializeField]
        protected CanvasGroup canvasGroup;
        
        [Header("Графика"), Space]
        [SerializeField]
        protected Image itemIconImage;
        [SerializeField]
        protected Image setImage;
            
        [Header("Текст"), Space]
        [SerializeField]
        protected TMP_Text amountText;
        [SerializeField]
        protected TMP_Text setNameText;
        [SerializeField]
        protected TMP_Text priceText;
        
        [Header("Параметры ячейки"), Space]
        [SerializeField]
        protected bool canDrag = true;
        [SerializeField]
        protected bool canDrop = true;

        protected InventoryDragDropCoordinator dropCoordinator;
        
        protected Blackset.Inventory.Inventories.Inventory inventory;
        protected string itemCellId;
        

        protected virtual void OnEnable()
        {
            dropCoordinator = InventoryDragDropCoordinator.Instance;
            if ( dropCoordinator != null ) Initialize();
            NotifyInitialized();
        }
        
        #region Drag'n'Drop

        public void OnBeginDrag(PointerEventData eventData)
        {
            if ( !IsInitialized || !canDrag ) return;
            itemIconImage.CrossFadeAlpha(0.25f, 0.1f, false);
            dropCoordinator.BeginDrag(inventory, itemCellId);
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

            dropCoordinator.RequestDrop(inventory, itemCellId);
        }

        #endregion

        /// <summary>
        /// Инициализация элемента
        /// </summary>
        /// <param name="newItemCellId">Идентификатор хранимых данных</param>
        public void InitializeElement(Blackset.Inventory.Inventories.Inventory newInventory, string newItemCellId = null)
        {
            if (newInventory == null)
            {
                ServiceDebug.LogError("Ссылка на инвентарь отсутствует, инициализация прервана");
                return;
            }
            
            inventory = newInventory;
            itemCellId = newItemCellId;
            
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
                InventoryCell cell = newInventory.GetById(newItemCellId);
                
                if (cell == null)
                {
                    ServiceDebug.LogError($"Ячейка с идентификатором «{newItemCellId}» не найдена, инициализация провалена");
                    return;
                }
                
                InventoryItem cellItem = newInventory.GetCellItemData(cell);
                InventoryItemType cellItemTypeData = newInventory.GetCellTypeData(cell);
                
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
        
        #endregion
    }
}