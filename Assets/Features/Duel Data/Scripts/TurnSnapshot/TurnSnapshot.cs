using System;
using System.Collections.Generic;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Duel.Rolls;
using Extensions.Helpers;

namespace Blackset.Duel.Snapshots
{
    /// <summary>
    /// Рабочий снапшот текущего броска
    /// </summary>
    public class TurnSnapshot : ICloneable<TurnSnapshot>
    {
        #region Данные снапшота
        
        /// <summary>
        /// Счета битвы участников дуэли [идентификатор, счет]
        /// </summary>
        public Dictionary<string, int> ParticipantScores { get; }
        /// <summary>
        /// Очки участников за дуэль [идентификатор, очки]
        /// </summary>
        public Dictionary<string, int> ParticipantDuelScores { get; }
        /// <summary>
        /// Намерения участников дуэли [идентификатор, намерение]
        /// </summary>
        public Dictionary<string, TurnParticipantState> ParticipantStates { get; }
        /// <summary>
        /// Знания об участниках дуэли [идентификатор, знания]
        /// </summary>
        public Dictionary<string, KnowledgeState> ParticipantKnowledge { get; }
        /// <summary>
        /// Текущие броски участников [идентификатор, запись броска]
        /// </summary>
        public Dictionary<string, RollHistoryEntry> ParticipantCurrentRolls { get; }

        /// <summary>
        /// Изменения состояний использования источников по итогам текущего броска
        /// </summary>
        public List<UsageMutation> UsageMutations { get; }
        
        #endregion

        #region Конструкторы
        
        /// <summary>
        /// Новый пустой снапшот
        /// </summary>
        private TurnSnapshot()
        {
            ParticipantScores = new Dictionary<string, int>();
            ParticipantStates = new Dictionary<string, TurnParticipantState>();
            ParticipantKnowledge = new Dictionary<string, KnowledgeState>();
            ParticipantCurrentRolls = new Dictionary<string, RollHistoryEntry>();
            UsageMutations = new List<UsageMutation>();
            ParticipantDuelScores = new Dictionary<string, int>();
        }

        /// <summary>
        /// Новый снапшот текущего броска
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        public TurnSnapshot(DuelContext context)
        {
            ParticipantScores = new Dictionary<string, int>();
            ParticipantStates = new Dictionary<string, TurnParticipantState>();
            ParticipantKnowledge = new Dictionary<string, KnowledgeState>();
            ParticipantCurrentRolls = new Dictionary<string, RollHistoryEntry>();
            UsageMutations = new List<UsageMutation>();
            ParticipantDuelScores = new Dictionary<string, int>();

            foreach (var participant in context.Participants)
            {
                ParticipantScores.Add(participant.Key, participant.Value.FightState.FightScore.Value);
                ParticipantStates.Add(participant.Key, participant.Value.FightState.TurnState.Clone());
                ParticipantKnowledge.Add(participant.Key, context.Knowledge[participant.Key].Clone());
                ParticipantDuelScores.Add(participant.Key, participant.Value.DuelScore.Value);
            }
        }
        
        #endregion

        #region Манипуляция данными
        
        /// <summary>
        /// Изменить счет участника
        /// </summary>
        public void AddScore(string participantId, int delta)
        {
            if (!ParticipantScores.ContainsKey(participantId))
            {
                ParticipantScores.Add(participantId, 0);
            }

            ParticipantScores[participantId] += delta;
        }
        
        /// <summary>
        /// Изменить очки дуэли участника
        /// </summary>
        public void AddDuelScore(string participantId, int delta)
        {
            if (!ParticipantDuelScores.ContainsKey(participantId))
            {
                ParticipantDuelScores.Add(participantId, 0);
            }

            ParticipantDuelScores[participantId] += delta;
        }
        
        /// <summary>
        /// Установить запись текущего броска участника
        /// </summary>
        public void SetCurrentRoll(string participantId, RollHistoryEntry rollEntry)
        {
            ParticipantCurrentRolls[participantId] = rollEntry;
        }

        /// <summary>
        /// Попробовать получить запись текущего броска участника
        /// </summary>
        public bool TryGetCurrentRoll(string participantId, out RollHistoryEntry rollEntry)
        {
            return ParticipantCurrentRolls.TryGetValue(participantId, out rollEntry);
        }

        /// <summary>
        /// Добавить изменение использования источника
        /// </summary>
        public void AddUsageMutation(UsageMutation usageMutation)
        {
            UsageMutations.Add(usageMutation);
        }

        #endregion

        #region Клонирование
        
        public TurnSnapshot CloneDeep()
        {
            var clone = new TurnSnapshot();

            foreach (var (key, value) in ParticipantScores)
                clone.ParticipantScores.Add(key, value);
            
            foreach (var (key, value) in ParticipantDuelScores)
                clone.ParticipantDuelScores.Add(key, value);

            foreach (var (key, value) in ParticipantStates)
                clone.ParticipantStates.Add(key, value.Clone());

            foreach (var (key, value) in ParticipantKnowledge)
                clone.ParticipantKnowledge.Add(key, value.Clone());

            foreach (var (key, value) in ParticipantCurrentRolls)
            {
                clone.ParticipantCurrentRolls.Add(
                    key,
                    new RollHistoryEntry(value.ThrowIndex, value.DiceInstanceId, value.RawResult)
                    {
                        FinalResult = value.FinalResult
                    });
            }

            foreach (UsageMutation mutation in UsageMutations)
            {
                clone.UsageMutations.Add(new UsageMutation(
                    mutation.OwnerParticipantId,
                    mutation.SourceInstanceId,
                    mutation.SourceKind,
                    mutation.TargetParticipantId));
            }

            return clone;
        }

        public TurnSnapshot CloneShallow() => throw new NotImplementedException();
        
        #endregion
    }
}