using Blackset.Data.Items.Types;
using Blackset.Inventories.Cells;
using Features.Inventory.Scripts.Items;

namespace Blackset.Data
{
    /// <summary>
    /// Структура данных определенного расходника
    /// </summary>
    public struct ConsumableItemContext
    {
        public ConsumableData Consumable;
        public ConsumableType Type;
        //public BaseItemRarity Rarity;
        
        public ItemContext ToItemContext() => new ItemContext(Consumable.Id, Type.Id, ItemClass.Consumable);
    }
}