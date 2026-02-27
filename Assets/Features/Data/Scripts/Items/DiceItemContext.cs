using Blackset.Data.Items.Types;

namespace Blackset.Data
{
    /// <summary>
    /// Структура данных определенного дайса
    /// </summary>
    public struct DiceItemContext
    {
        /// <summary>
        /// Игровые данные особого дайса
        /// </summary>
        public DiceData Dice;
        /// <summary>
        /// Тип дайса
        /// </summary>
        public DiceType Type;
        // TODO редкость: public BaseItemRarity Rarity;
    }
}