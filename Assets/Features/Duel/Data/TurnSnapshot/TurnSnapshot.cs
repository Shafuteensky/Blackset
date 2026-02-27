using System.Collections.Generic;
using Blackset.Duel.Participants;
using Blackset.Duel.Rules;
using Blackset.Duel.TurnIntents;
using Extensions.Helpers;

namespace Blackset.Duel.Snapshots
{
    /// <summary>
    /// Снапшот хода боя
    /// </summary>
    public struct TurnSnapshot
    {
        /// <summary>
        /// Правила дуэли
        /// </summary>
        /// </summary>
        public DuelRulesConfiguration Rules { get; }
        /// <summary>
        /// Целевое значение
        /// </summary>
        public int TargetValue { get; }
        
        /// <summary>
        /// Счета участников дуэли <идентификатор, счет>
        /// </summary>
        public Dictionary<string, int> ParticipantScores { get; }
        
        /// <summary>
        /// Намерения участников дуэли <идентификатор, намерение>
        /// </summary>
        public Dictionary<string, TurnIntent> ParticipantIntents { get; }
        /// <summary>
        /// Знания об участниках дуэли <идентификатор, знания>
        /// </summary>
        public Dictionary<string, KnowledgeState> PlayerKnowledge { get; }

        /// <summary>
        /// Новый снапшот
        /// </summary>
        /// <param name="rules">Конфигурация правил дуэли с учетом штормов</param>
        /// <param name="targetValue">Целевое значение</param>
        /// <param name="participantScores">Счета участников за бой</param>
        /// <param name="participantIntents">Намерения участников на ход</param>
        /// <param name="participantsKnowledge">Состояние знаний об участнике (знания об этом участнике со стороны других)</param>
        public TurnSnapshot(
            DuelRulesConfiguration rules,
            int targetValue, 
            Dictionary<string, int> participantScores,
            Dictionary<string, TurnIntent> participantIntents,
            Dictionary<string, KnowledgeState> participantsKnowledge)
        {
            Rules = rules;
            TargetValue = targetValue;
            ParticipantScores = CollectionCopy.DictionaryShallow(participantScores);
            ParticipantIntents = CollectionCopy.DictionaryShallow(participantIntents);
            PlayerKnowledge = CollectionCopy.DictionaryShallow(participantsKnowledge);
        }
    }
}