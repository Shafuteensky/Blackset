using System.Collections.Generic;
using Blackset.Data.Items.Types;

namespace Blackset.Duel.Context
{
    /// <summary>
    /// Данные о целевом значении дуэли
    /// </summary>
    public struct DuelTargetValueContext
    {
        /// <summary>
        /// Целевое значение
        /// </summary>
        public int TargetValue;
        /// <summary>
        /// Броски-источники результатов, составивших ЦЗ
        /// </summary>
        public Dictionary<DiceType, int> SourceRolls;
    }
}