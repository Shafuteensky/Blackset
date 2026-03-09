using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Duel.Rules;

namespace Blackset.Duel.Requests
{
    /// <summary>
    /// Запрос на заполнение пулов участников дуэли
    /// </summary>
    public struct PoolBuildRequest
    {
        /// <summary>
        /// Правила дуэли
        /// </summary>
        public DuelRulesConfiguration DuelRules { get; }
        /// <summary>
        /// Пул дайсов по типам
        /// </summary>
        public Dictionary<DiceType, List<DiceItemContext>> DicesPool { get; }
        /// <summary>
        /// Пул расходников
        /// </summary>
        public List<ConsumableItemContext> ConsumablesPool { get; }
        
        /// <summary>
        /// Запрос на заполнение пулов участников дуэли
        /// </summary>
        /// <param name="duelRules">Правила дуэли</param>
        /// <param name="dices">Список дайсов — группируется по типу автоматически</param>
        /// <param name="consumablesPool">Пул расходников</param>
        public PoolBuildRequest(DuelRulesConfiguration duelRules, 
            List<DiceItemContext> dices, List<ConsumableItemContext> consumablesPool)
        {
            DuelRules = duelRules;
            ConsumablesPool = consumablesPool;

            DicesPool = new Dictionary<DiceType, List<DiceItemContext>>();
            foreach (DiceItemContext dice in dices)
            {
                if (!DicesPool.TryGetValue(dice.GetDiceType(), out List<DiceItemContext> list))
                {
                    list = new List<DiceItemContext>();
                    DicesPool[dice.GetDiceType()] = list;
                }
                list.Add(dice);
            }
        }
    }
}