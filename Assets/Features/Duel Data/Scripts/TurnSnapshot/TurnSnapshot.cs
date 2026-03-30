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
            ParticipantCurrentRolls = new Dictionary<string, RollHistoryEntry>();
            UsageMutations = new List<UsageMutation>();
        }

        /// <summary>
        /// Новый снапшот текущего броска
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        public TurnSnapshot(DuelContext context)
        {
            ParticipantScores = new Dictionary<string, int>();
            ParticipantCurrentRolls = new Dictionary<string, RollHistoryEntry>();
            UsageMutations = new List<UsageMutation>();

            foreach (var participant in context.Participants)
            {
                ParticipantScores.Add(participant.Key, participant.Value.FightState.FightScore.Value);
            }
        }
        
        #endregion

        #region Манипуляция данными
        
        /// <summary>
        /// Пересчитать счета битвы участников по актуальным результатам их дайсов
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        public void RecalculateFightScores()
        {
            foreach (var participantPair in ParticipantCurrentRolls)
            {
                string participantId = participantPair.Key;
                RollHistoryEntry participant = participantPair.Value;
                
                ParticipantScores[participantId] = 

                Dictionary<string, int> latestDiceResults = new Dictionary<string, int>();

                foreach (RollHistoryEntry rollEntry in participant.FightState.RollHistory.Values)
                {
                    latestDiceResults[rollEntry.DiceInstanceId] = rollEntry.FinalResult;
                }

                if (ParticipantCurrentRolls.TryGetValue(participantId, out RollHistoryEntry currentRoll))
                {
                    latestDiceResults[currentRoll.DiceInstanceId] = currentRoll.FinalResult;
                }

                int totalScore = 0;
                foreach (int rollResult in latestDiceResults.Values)
                {
                    totalScore += rollResult;
                }

                ParticipantScores[participantId] = totalScore;
            }
        }
        
        /// <summary>
        /// Изменить счет участника
        /// </summary>
        public void AddScore(string participantId, int delta)
        {
            ParticipantScores.TryAdd(participantId, 0);
            ParticipantScores[participantId] += delta;
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