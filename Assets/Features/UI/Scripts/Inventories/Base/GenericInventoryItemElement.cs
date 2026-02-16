using System;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;
using Extensions.Generics;
using Extensions.Helpers;
using Extensions.Log;
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
        where TData : BaseData
        where TItemType : BaseItemType
    {
        [Header("Вывод"), Space]
        [SerializeField]
        protected Image itemIconImage;

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
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!IsInitialized) return;
            dropCoordinator.UpdatePosition(eventData.position);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsInitialized) return;
            itemIconImage.CrossFadeAlpha(0.25f, 0.1f, false);
            dropCoordinator.BeginDrag(inventory, itemCellId);
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
        public void InitializeElement(TInventory inventory, string itemCellId)
        {
            if (inventory == null || String.IsNullOrEmpty(itemCellId))
            {
                ServiceDebug.LogError("Инициализация прервана: переданы неполные данные");
                return;
            }
            
            this.inventory = inventory;
            this.itemCellId = itemCellId;
            
            TItemCell cell = inventory.GetById(itemCellId);
            if (cell != null && (cell.IsDefault || cell.IsEmpty))
            {
                canDrag = false;
                canDrop = true;
            }
            SetIcon();
        }

        private void SetIcon()
        {
            if ( String.IsNullOrEmpty(itemCellId) )
            {
                ServiceDebug.LogError($"Идентификатор «{itemCellId}» невалиден, иконка не назначена");
                return;
            }
            TItemCell cell = inventory.GetById(itemCellId);
            
            if ( cell == null )
            {
                ServiceDebug.LogError($"Ячейка с идентификатором «{itemCellId}» не найдена, иконка не назначена");
                return;
            }
            TItemType itemType = cell.GetTypeData(inventory.TypeRegistry);
            
            if (itemIconImage == null || itemType == null || itemType.Icon == null)
            {
                ServiceDebug.LogError("Информация об иконке отсутствует, иконка не назначена");
                return;
            }

            itemIconImage.sprite = itemType.Icon;
        }
    }
}