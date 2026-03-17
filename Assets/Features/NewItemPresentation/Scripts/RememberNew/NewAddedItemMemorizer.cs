using Blackset.Data.Registries;
using Blackset.Inventories;

namespace Blackset.Shop
{
    /// <summary>
    /// Контроллер учета новых купленных секретных предметов
    /// </summary>
    public class NewAddedItemMemorizer : AbstractRememberNewItemsController
    {
        private Inventory playerInventory;
        
        protected override void Awake()
        {
            base.Awake();
            
            playerInventory = GameData.Instance.PlayerDataFacade.Inventory;
        }
        
        private void OnEnable() => playerInventory.onItemAdded += RememberNewItem;

        private void OnDisable() => playerInventory.onItemAdded -= RememberNewItem;
    }
}