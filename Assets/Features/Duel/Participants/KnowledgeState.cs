using System.Collections.Generic;
using Blackset.Data.Items.Types;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Состояние знаний об участнике (знания об этом участнике со стороны других)
    /// </summary>
    public struct KnowledgeState
    {
        /// <summary>
        /// Список открытых за дуэль дайсов
        /// </summary>
        public List<DiceType> RevealedDices;
    }
}