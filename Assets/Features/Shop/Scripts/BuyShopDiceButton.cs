using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;

namespace Features.Shop
{
    /// <summary>
    /// Кнопка покупки дайса в магазине
    /// </summary>
    public class BuyShopDiceButton : GenericBuyShopItemButton<DicesInventory, DiceItemCell, DiceData, DiceType> { }
}