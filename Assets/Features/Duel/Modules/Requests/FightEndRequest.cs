using System.Collections.Generic;
using Blackset.Duel.Rules;

namespace Features.Duel.Requests
{
    /// <summary>
    /// Запрос на завершение боя
    /// </summary>
    public class FightEndRequest
    {
        /// <summary>
        /// Правила дуэли
        /// </summary>
        public DuelRulesConfiguration DuelRules { get; }
        /// <summary>
        /// Целевое значение
        /// </summary>
        public int TargetValue { get; } // TODO заменить на TargetValueContext?
        /// <summary>
        /// Количество совершенных ходов (бросков)
        /// </summary>
        public int ThrowNumber { get; }
        /// <summary>
        /// Счета игроков [идентификатор_иучастника, счет]
        /// </summary>
        public Dictionary<string, int> ParticipantScores { get; }
        /// <summary>
        /// Сдался ли участник [идентификатор_иучастника, состояние_сдачи]
        /// </summary>
        public Dictionary<string, bool> ParticipantGaveUpStates { get; }

        /// <summary>
        /// Запрос на завершение боя
        /// </summary>
        /// <param name="duelRules">Правила дуэли</param>
        /// <param name="targetValue">Целевое значение</param>
        /// <param name="throwNumber">Количество совершенных ходов (бросков)</param>
        /// <param name="participantScores">Счета игроков [идентификатор_иучастника, счет]</param>
        /// <param name="participantGaveUpStates">Сдался ли участник [идентификатор_иучастника, состояние_сдачи]</param>
        public FightEndRequest(DuelRulesConfiguration duelRules, int targetValue, int throwNumber, 
            Dictionary<string, int> participantScores, Dictionary<string, bool> participantGaveUpStates)
        {
            DuelRules = duelRules;
            TargetValue = targetValue;
            ThrowNumber = throwNumber;
            ParticipantScores = participantScores;
            ParticipantGaveUpStates = participantGaveUpStates;
        }
    }
}