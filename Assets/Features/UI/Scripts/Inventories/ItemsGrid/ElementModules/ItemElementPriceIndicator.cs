using Blackset.Shop;
using TMPro;
using UnityEngine;

namespace Blackset.UI.InventoryManagement.ElementModules
{
    /// <summary>
    /// Индикатор стоимости предмета
    /// </summary>
    [RequireComponent(typeof(InventoryItemElement))]
    public class ItemElementPriceIndicator : BaseInventoryItemModule
    {
        [Header("Стоимость"), Space]
        [SerializeField] private GameObject priceIndicator;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private BuyShopItemButton buyButton;

        protected override void OnInitialized()
        {
            if (priceIndicator != null) priceIndicator.SetActive(true);
            
            if (priceText != null)
            {
                int cellItemPrice;

                if (buyButton != null && buyButton.IsFixedPrice) 
                    cellItemPrice = ShopController.GetFixedPrice(itemElement.CellItem.ItemClass);
                else 
                    cellItemPrice = itemElement.CellItem.GetPrice(itemElement.CellItemType);
                
                priceText.text = cellItemPrice.ToString();
            }
        }
    }
}