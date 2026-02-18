using System;
using Blackset.Data.Items.Types;
using Blackset.Effects;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;
using Extensions.Generics;
using Extensions.Log;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Элемент UI фабрики содержимого инвентаря
    /// </summary>
    public abstract class GenericInventoryItemElement<TInventory, TItemCell, TData, TItemType> : InitializableMonoBehaviour, 
        IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler
        where TInventory : BaseInventory<TItemCell, TData, TItemType>
        where TItemCell : BaseItemCell<TData, TItemType>
        where TData : EffectingItemData
        where TItemType : BaseItemType
    {
        /// <summary>
        /// Canvas-группа
        /// </summary>
        public CanvasGroup CanvasGroup => canvasGroup;
        
        /// <summary>
        /// Прилинкованный инвентарь элемента
        /// </summary>
        public TInventory Inventory => inventory;
        /// <summary>
        /// Идентификатор прилинкованной ячейки инвентаря
        /// </summary>
        public string ItemCellId => itemCellId;
        
        [SerializeField]
        protected CanvasGroup canvasGroup;
        
        [Header("Вывод"), Space]
        [SerializeField]
        protected Image itemIconImage;
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

        protected GenericInventoryDragDropCoordinator<TInventory, TItemCell, TData, TItemType> dropCoordinator;
        
        protected TInventory inventory;
        protected string itemCellId;
        

        protected virtual void OnEnable()
        {
            dropCoordinator = GenericInventoryDragDropCoordinator<TInventory, TItemCell, TData, TItemType>.Instance;
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
        public void InitializeElement(TInventory inventory, string itemCellId = null)
        {
            if (inventory == null)
            {
                ServiceDebug.LogError("Ссылка на инвентарь отсутствует, инициализация прервана");
                return;
            }
            
            this.inventory = inventory;
            this.itemCellId = itemCellId;
            
            bool isNoCell = String.IsNullOrEmpty(itemCellId);
            
            // Дроп-зона, не конкретная ячейка
            if (isNoCell)
            {
                canDrag = false;
                canDrop = true;
            }
            // Конкретная ячейка
            else
            {
                TItemCell cell = inventory.GetById(itemCellId);
                
                if (cell == null)
                {
                    ServiceDebug.LogError($"Ячейка с идентификатором «{itemCellId}» не найдена, инициализация провалена");
                    return;
                }
                
                TData cellItemData = inventory.GetCellItemData(cell);
                TItemType cellItemTypeData = inventory.GetCellTypeData(cell);
                
                if (cellItemTypeData == null || cellItemData == null)
                {
                    ServiceDebug.LogError("Данные ячейки не полные, инициализация провалена");
                    return;
                }
                
                SetDragDropConfig(cell);
                SetIcon(cell, cellItemData, cellItemTypeData);
                
                SetAmountText(cell);
                SetItemSetText(cellItemData);
                SetItemPriceText(cellItemData, cellItemTypeData);
            }
        }

        #region Internal
        
        private void SetDragDropConfig(TItemCell cell)
        {
            if ( String.IsNullOrEmpty(itemCellId) || ( cell != null && (cell.IsDefault || cell.IsEmpty) ) )
            {
                canDrag = false;
                canDrop = true;
            }
        }

        private void SetIcon(TItemCell cell, TData item, TItemType itemType)
        {
            itemIconImage.sprite = itemType.Icon;
            if (cell.IsDefault)
            {
                Color newColor = item.Color;
                newColor.a = 0.5f;
                itemIconImage.color = newColor;
            }
            else
                itemIconImage.color = item.Color;
        }

        private void SetAmountText(TItemCell cell)
        {
            if (amountText != null)
            {
                if ( cell is {ItemAmount: > 1} )
                    amountText.text = $"x{cell.ItemAmount}";
                else
                    amountText.text = String.Empty;
            }
        }

        private void SetItemSetText(TData cellItemData)
        {
            if (setNameText != null) setNameText.text = cellItemData.Set.DataName;
        }

        private void SetItemPriceText(TData cellItemData, TItemType cellItemTypeData)
        {
            if (priceText != null)
            {
                int cellItemPrice = cellItemData.GetPrice(cellItemTypeData);
                priceText.text = cellItemPrice.ToString();
            }
        }
        
        #endregion
    }
}