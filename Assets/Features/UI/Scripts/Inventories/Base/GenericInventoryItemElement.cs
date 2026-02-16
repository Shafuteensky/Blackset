using System;
using Blackset.Data.Base;
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
        
        [SerializeField]
        protected CanvasGroup canvasGroup;
        
        [Header("Вывод"), Space]
        [SerializeField]
        protected Image itemIconImage;
        [SerializeField]
        protected TMP_Text amountText;

        protected GenericInventoryDragDropCoordinator<TInventory, TItemCell, TData, TItemType> dropCoordinator;
        
        protected TInventory inventory;
        protected string itemCellId;
        
        protected bool canDrag = true;
        protected bool canDrop = true;

        protected virtual void OnEnable()
        {
            dropCoordinator = GenericInventoryDragDropCoordinator<TInventory, TItemCell, TData, TItemType>.Instance;
            if (dropCoordinator != null) Initialize();
            NotifyInitialized();
        }
        
        #region Drag'n'Drop

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsInitialized) return;
            itemIconImage.CrossFadeAlpha(0.25f, 0.1f, false);
            dropCoordinator.BeginDrag(inventory, itemCellId);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!IsInitialized) return;
            dropCoordinator.UpdatePosition(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsInitialized) return;
            itemIconImage.CrossFadeAlpha(1, 0.1f, false);
            dropCoordinator.EndDrag();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!IsInitialized) return;

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
                
                if ( String.IsNullOrEmpty(itemCellId) || (cell != null && (cell.IsDefault || cell.IsEmpty)) )
                {
                    canDrag = false;
                    canDrop = true;
                }
                SetIcon();

                if ( amountText != null )
                {
                    if ( cell is { ItemAmount: > 1 } )
                        amountText.text = $"x{cell.ItemAmount}";
                    else
                        amountText.text = String.Empty;
                }
            }
        }

        private void SetIcon()
        {
            TItemCell cell = inventory.GetById(itemCellId);
            if ( cell == null )
            {
                ServiceDebug.LogError($"Ячейка с идентификатором «{itemCellId}» не найдена, иконка не назначена");
                return;
            }
            TData item = cell.GetItemData(inventory.DataRegistry);
            TItemType itemType = cell.GetTypeData(inventory.TypeRegistry);
            
            if (itemIconImage == null || itemType == null || itemType.Icon == null)
            {
                ServiceDebug.LogError("Информация об иконке отсутствует, иконка не назначена");
                return;
            }

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
    }
}