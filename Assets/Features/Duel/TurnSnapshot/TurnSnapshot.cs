using System.Collections.Generic;
using Blackset.Duel.Participants;
using Blackset.Duel.Rules;
using Blackset.Duel.TurnIntents;

namespace Blackset.Duel.TurnSnapshot
{
    /// <summary>
    /// Снапшот хода боя
    /// </summary>
    public struct TurnSnapshot
    {
        /// <summary>
        /// Правила дуэли
        /// </summary>
        public DuelRulesConfig Rules;
        /// <summary>
        /// Целевое значение
        /// </summary>
        public int TargetValue;
        
        /// <summary>
        /// Счета участников дуэли <идентификатор, счет>
        /// </summary>
        public Dictionary<string, int> ParticipantScores;
        
        /// <summary>
        /// Намерения участников дуэли <идентификатор, намерение>
        /// </summary>
        public Dictionary<string, TurnIntent> ParticipantIntents;
        /// <summary>
        /// Знания об участниках дуэли <идентификатор, знания>
        /// </summary>
        public Dictionary<string, KnowledgeState> PlayerKnowledge;
    }
}