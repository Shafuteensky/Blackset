using System;
using System.Collections.Generic;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Extensions.Helpers;

namespace Blackset.Duel.Snapshots
{
    /// <summary>
    /// Снапшот хода боя
    /// </summary>
    public class TurnSnapshot : ICloneable<TurnSnapshot>
    {
        /// <summary>
        /// Счета участников дуэли [идентификатор, счет]
        /// </summary>
        public Dictionary<string, int> ParticipantScores { get; }
        
        // TODO FightState для RawRolls
        /// <summary>
        /// Намерения участников дуэли [идентификатор, намерение]
        /// </summary>
        public Dictionary<string, TurnParticipantState> ParticipantStates { get; }
        /// <summary>
        /// Знания об участниках дуэли [идентификатор, знания]
        /// </summary>
        public Dictionary<string, KnowledgeState> ParticipantKnowledge { get; }
        /// <summary>
        /// Сырые результаты бросков дайсов [id_участника, [id_дайса_в_сборке, результат]]
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> ParticipantRawRollResults { get; }
        
        /// <summary>
        /// Новый пустой снапшот
        /// </summary>
        private TurnSnapshot()
        {
            ParticipantScores = new Dictionary<string, int>();
            ParticipantStates = new Dictionary<string, TurnParticipantState>();
            ParticipantKnowledge = new Dictionary<string, KnowledgeState>();
            ParticipantRawRollResults = new Dictionary<string, Dictionary<string, int>>();
        }
        
        /// <summary>
        /// Новый снапшот
        /// </summary>
        /// <param name="context">Данные дуэли/param>
        public TurnSnapshot(DuelContext context)
        {
            ParticipantScores = new Dictionary<string, int>();
            ParticipantStates = new Dictionary<string, TurnParticipantState>();
            ParticipantKnowledge = new Dictionary<string, KnowledgeState>();
            ParticipantRawRollResults = new Dictionary<string, Dictionary<string, int>>();
            
            foreach (var participant in context.Participants)
            {
                ParticipantScores.Add(participant.Key, participant.Value.FightState.Score.Value);
                ParticipantStates.Add(participant.Key, participant.Value.FightState.TurnState.Clone());
                ParticipantKnowledge.Add(participant.Key, context.Knowledge[participant.Key].Clone());
                ParticipantRawRollResults.Add(participant.Key,
                    new Dictionary<string, int>(
                        context.Participants[participant.Key].FightState.RawRollResults));
            }
        }
        
        public TurnSnapshot CloneDeep()
        {
            var clone = new TurnSnapshot();

            foreach (var (key, value) in ParticipantScores)
                clone.ParticipantScores.Add(key, value);

            foreach (var (key, value) in ParticipantStates)
                clone.ParticipantStates.Add(key, value.Clone());

            foreach (var (key, value) in ParticipantKnowledge)
                clone.ParticipantKnowledge.Add(key, value.Clone());

            foreach (var (key, value) in ParticipantRawRollResults)
                clone.ParticipantRawRollResults.Add(key, new Dictionary<string, int>(value));

            return clone;
        }

        public TurnSnapshot CloneShallow() => throw new NotImplementedException();
    }
}