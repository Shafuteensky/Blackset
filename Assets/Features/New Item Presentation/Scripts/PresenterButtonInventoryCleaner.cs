using Blackset.Data.Registries;
using Blackset.Inventories;
using Extensions.Generics;

namespace Blackset.Shop
{
    /// <summary>
    /// Очистка презентационного инвентаря по нажатию на кнопку
    /// </summary>
    public sealed class PresenterButtonInventoryCleaner : AbstractButton
    {
        private Inventory presenterInventory;

        protected override void Awake()
        {
            base.Awake();
            GameData gameData = GameData.Instance;
            presenterInventory = gameData.NewItemsPresenterInventory;
        }
        
        public override void OnButtonClick()
        {
            presenterInventory.Clear();
        }
    }
}