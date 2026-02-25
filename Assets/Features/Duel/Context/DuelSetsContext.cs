using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;

namespace Blackset.Duel.Context
{
    /// <summary>
    /// Сборки участников
    /// </summary>
    public struct DuelSetsContext
    {
        /// <summary>
        /// Сборка дайсов
        /// </summary>
        public Dictionary<DiceType, DiceItemContext> DicesSet;
        /// <summary>
        /// Сборка расходников
        /// </summary>
        public List<ConsumableItemContext> ConsumablesSet;
    }
}