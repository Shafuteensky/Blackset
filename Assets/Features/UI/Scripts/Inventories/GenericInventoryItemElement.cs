using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;
using Extensions.Log;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Элемент UI фабрики содержимого инвентаря
    /// </summary>
    public abstract class GenericInventoryItemElement<TInventory, TItemCell, TData, TItemType> : MonoBehaviour, 
        IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler
        where TInventory : BaseInventory<TItemCell, TData, TItemType>
        where TItemCell : BaseItemCell<TData, TItemType>
        where TData : BaseData
        where TItemType : BaseItemType
    {
        [Header("Вывод"), Space]
        [SerializeField]
        protected Image itemIconImage;
        
        protected TInventory inventory;
        protected BaseDataRegistry<TItemType> typeRegistry; 
        protected BaseDataRegistry<TData> dataRegistry; 
        protected string itemCellId;
        
        #region Drag'n'Drop
        
        public void OnDrag(PointerEventData eventData)
        {
            //ServiceDebug.Log("Попытка драга");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ServiceDebug.Log($"{name}: начало драга");
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ServiceDebug.Log($"{name}: конец драга");
        }

        public void OnDrop(PointerEventData eventData)
        {
            ServiceDebug.Log($"{name}: попытка дропа");
        }

        #endregion

        /// <summary>
        /// Инициализация элемента
        /// </summary>
        /// <param name="newItemCellId">Идентификатор зранимых данных</param>
        public void Initialize(TInventory inventory, string itemCellId, 
            BaseDataRegistry<TItemType> typeRegistry, BaseDataRegistry<TData> dataRegistry)
        {
            this.itemCellId = itemCellId;
            this.inventory = inventory;
            this.typeRegistry = typeRegistry;
            this.dataRegistry = dataRegistry;
            
            if (itemIconImage != null && typeRegistry != null && dataRegistry != null)
            {
                TItemCell cell = inventory.GetById(itemCellId);
                TItemType itemType = cell.GetTypeData(typeRegistry);
                itemIconImage.sprite = itemType.Icon;
            }
        }
    }
}