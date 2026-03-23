using Blackset.Data.Items.Visual.Modules;
using Blackset.Duel.Sequence;
using Blackset.Inventories;
using Blackset.Inventories.Scripts.Items;
using Blackset.UI.InventoryManagement;
using Extensions.Log;
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

        [Header("Предмет"), Space]
        [SerializeField] protected ItemClass visualItemClass;
        
        [Header("Модули"), Space]
        [SerializeField] protected ItemPositionController positionController;
        [SerializeField] protected VisualItemInteractionBlocker interactionBlocker;
        
        protected InventoryItemElement itemElement;
        protected DuelController duelController;

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
        /// <param name="newItemCellId">Идентификатор новой ячейки (хранимых данных)</param>
        public virtual void Initialize(string itemId, string ownerParticipantId,
            Inventory inventory, string newItemCellId)
        {
            ItemId = itemId;
            OwnerParticipantId = ownerParticipantId;
            
            itemElement.Initialize(inventory, newItemCellId);
            
            duelController = DuelController.Instance;
            if (duelController == null)
            {
                ServiceDebug.LogError(
                    $"Не найден инстанс {nameof(DuelController)}, визуальный предмет не инициализирован");
                return;
            }

            bool isParticipantPlayer = DuelController.Instance.DuelContext.PlayerId == ownerParticipantId;
            interactionBlocker.Initialize(isParticipantPlayer);
            
            positionController.Initialize(DuelController.Instance, itemId, ownerParticipantId, transform, visualItemClass);
        }
    }
}