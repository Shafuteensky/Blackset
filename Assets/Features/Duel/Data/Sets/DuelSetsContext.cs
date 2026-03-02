using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Extensions.Helpers;
using Extensions.Log;

namespace Blackset.Duel.Sets
{
    /// <summary>
    /// Сбори участника дуэли
    /// </summary>
    public class DuelSetsContext
    {
        /// <summary>
        /// Сборка дайсов <идентификатор_на_дуэль, дайс> (id задается при создании сборки)
        /// </summary>
        public Dictionary<string, DiceItemContext> DicesSet => dicesSet;
        /// <summary>
        /// Сборка расходников <идентификатор_на_дуэль, расходник> (id задается при создании сборки)
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
        public bool TryGetDice(string id, out DiceItemContext dice)
        {
            ServiceGuard.NotNullOrEmpty(id, nameof(id));
            return dicesSet.TryGetValue(id, out dice);
        }

        /// <summary>
        /// Получить дайс определенного типа
        /// </summary>
        public bool TryGetDice(DiceType diceType, out DiceItemContext foundDice)
        {
            foreach (DiceItemContext dice in dicesSet.Values)
            {
                if (dice.Type != diceType) continue;
                foundDice = dice;
                return true;
            }

            foundDice = default;
            return false;
        }

        /// <summary>
        /// Получить расходник по идентификатору
        /// </summary>
        public bool TryGetConsumable(string id, out ConsumableItemContext consumable)
        {
            ServiceGuard.NotNullOrEmpty(id, nameof(id));
            return consumablesSet.TryGetValue(id, out consumable);
        }

        #endregion
    }
}