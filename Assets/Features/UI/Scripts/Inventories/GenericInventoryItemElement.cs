using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Inventory.Cells;
using Extensions.Log;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Элемент UI фабрики содержимого инвентаря
    /// </summary>
    public abstract class GenericInventoryItemElement<TItemCell, TData, TItemType> : MonoBehaviour, IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler
        where TItemCell : BaseItemCell<TData, TItemType>
        where TData : BaseData
        where TItemType : BaseItemType
    {
        [Header("Данные"), Space]
        [SerializeField]
        protected DiceTypeRegistry dataRegistry;
        
        [Header("Вывод"), Space]
        [SerializeField]
        protected Image itemIconImage;
        
        protected TItemCell itemCell;
        
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
        public void Initialize(TItemCell item)
        {
            itemCell = item;
            if (itemIconImage != null && dataRegistry != null && itemCell != null)
            {
                itemIconImage.sprite = dataRegistry.GetById(itemCell.ItemTypeId).Icon;
            }
        }
    }
}