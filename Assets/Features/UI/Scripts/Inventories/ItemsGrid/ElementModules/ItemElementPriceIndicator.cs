using Blackset.Effects;
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
                int cellItemPrice = 0;

                if (buyButton != null && buyButton.IsFixedPrice && itemElement.CellItem is EffectingItem effectingItem)
                    cellItemPrice = effectingItem.Set.GetBuyPrice();
                else 
                    cellItemPrice = itemElement.CellItem.GetBuyPrice(itemElement.CellItemType);
                
                priceText.text = cellItemPrice.ToString();
            }
        }
    }
}