namespace Blackset.Shop
{
    /// <summary>
    /// Контроллер учета новых купленных секретных предметов
    /// </summary>
    public class NewSecretItemMemorizer : AbstractRememberNewItemsController
    {
        private ShopController shopController;
        
        protected override void Awake()
        {
            base.Awake();
            
            shopController = ShopController.Instance;
        }
        
        private void OnEnable() => shopController.onSecretItemBought += RememberNewItem;

        private void OnDisable() => shopController.onSecretItemBought -= RememberNewItem;
    }
}