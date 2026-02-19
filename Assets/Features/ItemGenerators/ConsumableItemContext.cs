using Blackset.Data;
using Blackset.Data.Items.Types;

namespace Features.ItemGenerators
{
    /// <summary>
    /// Структура данных определенного расходника
    /// </summary>
    public struct ConsumableItemContext
    {
        public ConsumableItem Item;
        public ConsumableType Type;
        //public BaseItemRarity Rarity;
    }
}