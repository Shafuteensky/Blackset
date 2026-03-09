using Blackset.Data.Items.Types;
using Blackset.Data.Registries;

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
        public string Dice;
        /// <summary>
        /// Тип дайса
        /// </summary>
        public string Type;
        // TODO редкость: public BaseItemRarity Rarity;
        
        public DiceData GetDice() => GameData.Instance.GetDice(Dice);
        
        public DiceType GetDiceType() => GameData.Instance.GetDiceType(Type);
    }
}