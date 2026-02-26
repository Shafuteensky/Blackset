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
        /// Раскрытые дайсы <идентификатор, дайс> (использованные хоть раз за дуэль)
        /// </summary>
        public Dictionary<string, bool> DiceRevealStatuses;
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
        /// Статус раскрытия дайса (хотя бы раз использован за дуэль с момента сосздания сборки участника)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsDiceRevealed(string id)
        {
            bool isRevealed = DiceRevealStatuses[id];
            return isRevealed;
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