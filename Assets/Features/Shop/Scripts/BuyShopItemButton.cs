using Blackset.UI.InventoryManagement;
using Extensions.Generics;
using UnityEngine;

namespace Blackset.Shop
{
    /// <summary>
    /// Абстракция кнопки покупки предмета магазина
    /// </summary>
    /// <remarks>
    /// Покупаются сразу все предметы из ячейки (количество)
    /// </remarks>
    [RequireComponent(typeof(InventoryItemElement))]
    public sealed class BuyShopItemButton : AbstractHoldButton
    {
        /// <summary>
        /// Фиксированная ли стоимость у лота
        /// </summary>
        public bool IsFixedPrice => isFixedPrice;
        
        [Header("Параметры лота"), Space]
        
        [Tooltip("Является ли предмет секретным")]
        [SerializeField] private bool isHidden;
        
        [Tooltip("Опциональная фиксированная стоимость лота (false чтобы брать цену от предмета)")]
        [SerializeField] private bool isFixedPrice;
        
        private InventoryItemElement itemElement;
        private ShopController shopController;

        protected override void Awake()
        {
            base.Awake();
            itemElement = GetComponent<InventoryItemElement>();
            shopController = ShopController.Instance;
        }
        
        public override void OnButtonClick()
        {
            shopController.TryBuyItem(itemElement, isHidden, isFixedPrice);
        }
    }
}