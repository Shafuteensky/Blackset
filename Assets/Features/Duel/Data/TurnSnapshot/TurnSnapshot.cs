using System.Collections.Generic;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;

namespace Blackset.Duel.Snapshots
{
    /// <summary>
    /// Снапшот хода боя
    /// </summary>
    public struct TurnSnapshot
    {
        /// <summary>
        /// Счета участников дуэли [идентификатор, счет]
        /// </summary>
        public Dictionary<string, int> ParticipantScores { get; }
        
        /// <summary>
        /// Намерения участников дуэли [идентификатор, намерение]
        /// </summary>
        public Dictionary<string, TurnParticipantState> ParticipantStates { get; }
        /// <summary>
        /// Знания об участниках дуэли [идентификатор, знания]
        /// </summary>
        public Dictionary<string, KnowledgeState> PlayerKnowledge { get; }

        /// <summary>
        /// Новый снапшот
        /// </summary>
        /// <param name="context">Данные дуэли/param>
        public TurnSnapshot(DuelContext context)
        {
            ParticipantScores = new Dictionary<string, int>();
            ParticipantStates = new Dictionary<string, TurnParticipantState>();
            PlayerKnowledge = new Dictionary<string, KnowledgeState>();
            
            foreach (var participant in context.Participants)
            {
                ParticipantScores.Add(participant.Key, participant.Value.FightState.Score);
                ParticipantStates.Add(participant.Key, participant.Value.FightState.TurnState);
                PlayerKnowledge.Add(participant.Key, context.Knowledge[participant.Key].Clone());
            }
        }
    }
}