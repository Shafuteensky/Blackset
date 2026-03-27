using System;
using System.Collections.Generic;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Extensions.Helpers;

namespace Blackset.Duel.Snapshots
{
    /// <summary>
    /// Рабочий снапшот текущего броска
    /// </summary>
    public class TurnSnapshot : ICloneable<TurnSnapshot>
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
        public Dictionary<string, KnowledgeState> ParticipantKnowledge { get; }

        /// <summary>
        /// Сырые результаты текущего броска [id_участника, результат]
        /// </summary>
        public Dictionary<string, int> ParticipantRawRollResults { get; }

        /// <summary>
        /// Финальные результаты текущего броска [id_участника, результат]
        /// </summary>
        public Dictionary<string, int> ParticipantFinalRollResults { get; }

        /// <summary>
        /// Изменения состояний использования источников по итогам текущего броска
        /// </summary>
        public List<UsageMutation> UsageMutations { get; }

        /// <summary>
        /// Новый пустой снапшот
        /// </summary>
        private TurnSnapshot()
        {
            ParticipantScores = new Dictionary<string, int>();
            ParticipantStates = new Dictionary<string, TurnParticipantState>();
            ParticipantKnowledge = new Dictionary<string, KnowledgeState>();
            ParticipantRawRollResults = new Dictionary<string, int>();
            ParticipantFinalRollResults = new Dictionary<string, int>();
            UsageMutations = new List<UsageMutation>();
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
            ParticipantRawRollResults = new Dictionary<string, int>();
            ParticipantFinalRollResults = new Dictionary<string, int>();
            UsageMutations = new List<UsageMutation>();

            foreach (var participant in context.Participants)
            {
                ParticipantScores.Add(participant.Key, participant.Value.FightState.FightScore.Value);
                ParticipantStates.Add(participant.Key, participant.Value.FightState.TurnState.Clone());
                ParticipantKnowledge.Add(participant.Key, context.Knowledge[participant.Key].Clone());
            }
        }

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
        /// Установить сырой результат текущего броска участника
        /// </summary>
        public void SetRawRollResult(string participantId, int rawResult)
        {
            ParticipantRawRollResults[participantId] = rawResult;
        }

        /// <summary>
        /// Установить финальный результат текущего броска участника
        /// </summary>
        public void SetFinalRollResult(string participantId, int finalResult)
        {
            ParticipantFinalRollResults[participantId] = finalResult;
        }

        /// <summary>
        /// Добавить изменение использования источника
        /// </summary>
        public void AddUsageMutation(UsageMutation usageMutation)
        {
            UsageMutations.Add(usageMutation);
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
                clone.ParticipantRawRollResults.Add(key, value);

            foreach (var (key, value) in ParticipantFinalRollResults)
                clone.ParticipantFinalRollResults.Add(key, value);

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
    }
}