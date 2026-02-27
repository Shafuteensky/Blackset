using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Extensions.Helpers;

namespace Blackset.Duel.Context
{
    /// <summary>
    /// Сборки участников
    /// </summary>
    public class DuelSetsContext
    {
        /// <summary>
        /// Сборка дайсов <идентификатор, дайс> (id задается при создании сборки)
        /// </summary>
        public Dictionary<string, DiceItemContext> DicesSet => dicesSet;
        /// <summary>
        /// Сборка расходников <идентификатор, расходник> (id задается при создании сборки)
        /// </summary>
        public Dictionary<string, ConsumableItemContext> ConsumablesSet => consumablesSet;
        
        private readonly Dictionary<string, DiceItemContext> dicesSet = new();
        private readonly Dictionary<string, ConsumableItemContext> consumablesSet = new();

        /// <summary>
        /// Заполнить данные сборок участника
        /// </summary>
        /// <param name="dices">Список дайсов</param>
        /// <param name="consumables">Список расходников</param>
        public DuelSetsContext(
            List<DiceItemContext> dices,
            List<ConsumableItemContext> consumables)
        {
            string id;
            
            foreach (DiceItemContext dice in dices)
            {
                id = IdGenerator.NewGuid();
                dicesSet.Add(id, dice);
            }

            foreach (ConsumableItemContext consumable in consumables)
            {
                id = IdGenerator.NewGuid();
                consumablesSet.Add(id, consumable);
            }
        }

        #region Getters
        
        /// <summary>
        /// Получить дайс по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор дайса</param>
        /// <returns></returns>
        public DiceItemContext GetDice(string id)
        {
            DiceItemContext dice = dicesSet[id];
            return dice;
        }
        
        /// <summary>
        /// Получить дайс определенного типа
        /// </summary>
        /// <param name="diceType">Тип дайса</param>
        /// <returns></returns>
        public bool TryGetDice(DiceType diceType, out DiceItemContext foundDice)
        {
            foreach (DiceItemContext dice in DicesSet.Values)
            {
                if (dice.Type == diceType)
                {
                    foundDice = dice;
                    return true;
                }
            }
            
            foundDice = default;
            return false;
        }
        
        /// <summary>
        /// Получить расходник по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор расходника</param>
        /// <returns></returns>
        public ConsumableItemContext GetConsumable(string id)
        {
            ConsumableItemContext consumable = consumablesSet[id];
            return consumable;
        }
        
        #endregion
    }
}