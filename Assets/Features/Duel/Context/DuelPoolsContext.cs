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
        public Dictionary<DiceType, List<DiceItemContext>> DicesPool;
        /// <summary>
        /// Пул расходников
        /// </summary>
        public List<ConsumableItemContext> ConsumablesPool;
    }
}