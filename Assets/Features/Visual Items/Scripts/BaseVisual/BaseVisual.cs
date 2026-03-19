using Blackset.Inventories;
using Blackset.UI.InventoryManagement;
using UnityEngine;

namespace Blackset.Data.Items.Visual
{
    /// <summary>
    /// Абстракция визуального представления предмета
    /// </summary>
    [RequireComponent(typeof(InventoryItemElement))]
    [RequireComponent(typeof(InventoryCellHoverInfoEmitter))]
    public abstract class BaseVisual : MonoBehaviour
    {
        /// <summary>
        /// Идентификатор предмета в сборке участника
        /// </summary>
        public string ItemId { get; private set; }
        /// <summary>
        /// Идентификатор хозяина-участника дуэли
        /// </summary>
        public  string OwnerParticipantId { get; private set; }

        protected InventoryItemElement itemElement;

        protected virtual void Awake()
        {
            itemElement = GetComponent<InventoryItemElement>();
        }
        
        /// <summary>
        /// Инициализация данных
        /// </summary>
        /// <param name="itemId">Идентификатор предмета в сборке участника</param>
        /// <param name="ownerParticipantId">Идентификатор участника-хозяина</param>
        /// <param name="inventory">Инвентарь (хранилище данных)</param>
        /// <param name="newItemCellId">Идентификатор нвой ячейки (хранимых данных)</param>
        public virtual void Initialize(string itemId, string ownerParticipantId,
            Inventory inventory, string newItemCellId)
        {
            ItemId = itemId;
            OwnerParticipantId = ownerParticipantId;
            
            itemElement.Initialize(inventory, newItemCellId);
        }
    }
}