using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;

namespace Blackset.Duel.Pools
{
    /// <summary>
    /// Пулы кубов и расходников участников дуэли
    /// </summary>
    public class DuelPoolsContext
    {
        /// <summary>
        /// Пул дайсов
        /// </summary>
        public Dictionary<DiceType, List<DiceItemContext>> DicesPool => dicesPool;
        /// <summary>
        /// Пул расходников
        /// </summary>
        public List<ConsumableItemContext> ConsumablesPool => consumablesPool;

        private readonly Dictionary<DiceType, List<DiceItemContext>> dicesPool = new();
        private readonly List<ConsumableItemContext> consumablesPool = new();

        /// <summary>
        /// Заполнить данные пулов участника
        /// </summary>
        /// <param name="dicesForPool">Список дайсов для пула</param>
        /// <param name="consumablesForPool">Список расходников для пула</param>
        public DuelPoolsContext(
            List<DiceItemContext> dicesForPool,
            List<ConsumableItemContext> consumablesForPool)
        {
            dicesPool = new Dictionary<DiceType, List<DiceItemContext>>();
            consumablesPool = consumablesForPool;

            foreach (DiceItemContext dice in dicesForPool)
            {
                if (!dicesPool.TryGetValue(dice.GetDiceType(), out var list))
                {
                    list = new List<DiceItemContext>();
                    dicesPool[dice.GetDiceType()] = list;
                }

                list.Add(dice);
            }
        }
        
        /// <summary>
        /// Заполнить данные пулов участника
        /// </summary>
        /// <param name="dicesForPool">Пулы дайсов</param>
        /// <param name="consumablesForPool">Список расходников для пула</param>
        public DuelPoolsContext(
            Dictionary<DiceType, List<DiceItemContext>> dicesForPool,
            List<ConsumableItemContext> consumablesForPool)
        {
            dicesPool = dicesForPool;
            consumablesPool = consumablesForPool;
        }
    }
}