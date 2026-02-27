using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;

namespace Blackset.Duel.Context
{
    /// <summary>
    /// Пулы кубов и расходников участников дуэли
    /// </summary>
    public struct DuelPoolsContext
    {
        /// <summary>
        /// Пул дайсов
        /// </summary>
        public Dictionary<DiceType, List<DiceItemContext>> DicesPool { get; private set; }
        /// <summary>
        /// Пул расходников
        /// </summary>
        public List<ConsumableItemContext> ConsumablesPool { get; private set; }

        /// <summary>
        /// Заполнить данные пулов участника
        /// </summary>
        /// <param name="dicesForPool">Список дайсов для пула</param>
        /// <param name="consumablesForPool">Список расходников для пула</param>
        public DuelPoolsContext(
            List<DiceItemContext> dicesForPool,
            List<ConsumableItemContext> consumablesForPool)
        {
            DicesPool = new Dictionary<DiceType, List<DiceItemContext>>();
            ConsumablesPool = consumablesForPool;

            foreach (DiceItemContext dice in dicesForPool)
            {
                if (!DicesPool.TryGetValue(dice.Type, out var list))
                {
                    list = new List<DiceItemContext>();
                    DicesPool[dice.Type] = list;
                }

                list.Add(dice);
            }
        }
    }
}