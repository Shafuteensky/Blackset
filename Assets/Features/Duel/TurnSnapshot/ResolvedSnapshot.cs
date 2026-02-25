using System.Collections.Generic;

namespace Blackset.Duel.TurnSnapshot
{
    /// <summary>
    /// Данные зарезолвенного снапшота
    /// </summary>
    public struct ResolvedSnapshot
    {
        /// <summary>
        /// Счета участников дуэли <идентификатор, счет>
        /// </summary>
        public Dictionary<string, int> ParticipantsScores;
    }
}