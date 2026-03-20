using Blackset.Data.Items.Visual;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Extensions.Data.InMemoryData;
using Extensions.Log;
using UnityEngine;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// Базовая фабрика визуальных представлений предметов инвентаря
    /// </summary>
    public abstract class BaseInventoryItemViewFactory<TVisual> : BaseInMemoryDataFactory<TVisual, InventoryCell, Inventory>
        where TVisual : BaseVisual
    {
        [Header("Принадлежность элементов"), Space]
        [SerializeField] private bool isOwnerPlayer;
        
        private DuelController duelController;

        protected void Awake()
        {
            duelController = DuelController.Instance;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            duelController.EventHub.Subscribe<SetReadyEvent>(RebuildOnSetReady);
        }
        
        private void RebuildOnSetReady(SetReadyEvent handler) => Rebuild();
        
        protected override void OnInstanceInitialization(TVisual instance, InventoryCell item, Inventory container)
        {
            ServiceGuard.NotNull(duelController, nameof(duelController));
            
            string ownerId;
            if (isOwnerPlayer) ownerId = duelController.DuelContext.PlayerId;
            else ownerId = duelController.DuelContext.OpponentId;
                
            instance.Initialize(item.Id, ownerId, container, item.Id);
        }
    }
}
