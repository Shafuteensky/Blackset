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
    /// UI фабрика визуальных представлений предметов инвентаря
    /// </summary>
    public class InventoryViewElementsFactory : BaseInMemoryDataFactory<VisualDice, InventoryCell, Inventory>
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
        
        protected override void OnInstanceInitialization(VisualDice instance, InventoryCell item, Inventory container)
        {
            ServiceGuard.NotNull(duelController, nameof(duelController));
            
            string ownerId;
            if (isOwnerPlayer) ownerId = duelController.DuelContext.PlayerId;
            else ownerId = duelController.DuelContext.OpponentId;
                
            instance.Initialize(item.Id, ownerId, container, item.Id);
        }
    }
}
