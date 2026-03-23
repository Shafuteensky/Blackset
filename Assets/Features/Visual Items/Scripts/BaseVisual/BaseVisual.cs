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
        public string OwnerParticipantId { get; private set; }

        [Header("Модули"), Space]
        [SerializeField] private BaseVisualItemModule[] visualModules;

        protected abstract ItemClass visualItemClass { get; }
        
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

            var context = new VisualItemContext(
                duelController,
                itemId,
                visualItemClass,
                ownerParticipantId,
                transform);

            if (visualModules == null) return;
            foreach (var module in visualModules)
                module.Initialize(context);
        }
    }
}