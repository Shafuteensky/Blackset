using Blackset.Data;
using Blackset.Data.Items.Types;

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
    }
}