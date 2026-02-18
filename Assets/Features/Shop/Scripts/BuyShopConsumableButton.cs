using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;

namespace Features.Shop
{
    /// <summary>
    /// Кнопка покупки расходника в магазине
    /// </summary>
    public class BuyShopConsumableButton : GenericBuyShopItemButton<ConsumablesInventory, ConsumableItemCell, ConsumableData, ConsumableType> { }
}