using System.Collections.Generic;
using Blackset.Data;

namespace Blackset.Duel.Context
{
    /// <summary>
    /// Сборки участников
    /// </summary>
    public struct DuelSetsContext
    {
        /// <summary>
        /// Сборка дайсов <идентификатор, дайс> (id задается при создании сборки)
        /// </summary>
        public Dictionary<string, DiceItemContext> DicesSet;
        /// <summary>
        /// Сборка расходников <идентификатор, расходник> (id задается при создании сборки)
        /// </summary>
        public Dictionary<string, ConsumableItemContext> ConsumablesSet;

        /// <summary>
        /// Получить дайс определенного типа
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DiceItemContext GetDice(string id)
        {
            DiceItemContext dice = DicesSet[id];
            return dice;
        }
        
        /// <summary>
        /// Получить расходник по индексу
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ConsumableItemContext GetConsumable(string id)
        {
            ConsumableItemContext consumable = ConsumablesSet[id];
            return consumable;
        }
    }
}