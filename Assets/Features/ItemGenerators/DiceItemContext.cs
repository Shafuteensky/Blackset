using Blackset.Data;
using Blackset.Data.Items.Types;

namespace Features.ItemGenerators
{
    /// <summary>
    /// Структура данных определенного дайса
    /// </summary>
    public struct DiceItemContext
    {
        public DiceData Item;
        public DiceType Type;
        //public BaseItemRarity Rarity;
    }
}