using System.Collections.Generic;
using Blackset.Duel.History;
using Blackset.Duel.Participants;
using Blackset.Duel.Rules;
using Blackset.DuelContracts;

namespace Blackset.Duel.Context
{
    /// <summary>
    /// Данные дуэли
    /// </summary>
    public struct DuelContext
    {
        /// <summary>
        /// Состояния активных эффектов
        /// </summary>
        // public EffectsStateContext effectsState;
        /// <summary>
        /// Правила дуэли
        /// </summary>
        public DuelRulesConfiguration Rules;
        /// <summary>
        /// Сид дуэли
        /// </summary>
        public int Seed;
        /// <summary>
        /// Активный контракт
        /// </summary>
        public DuelContract Contract;

        /// <summary>
        /// Участники <идентификатор, данные>
        /// </summary>
        public Dictionary<string, DuelParticipantState> Participants;
        /// <summary>
        /// Денные о целевом значении
        /// </summary>
        public TargetValueContext TargetValue;
        /// <summary>
        /// Данные о прогрессе дуэли
        /// </summary>
        public DuelProgressContext Progress;
        /// <summary>
        /// История ходов
        /// </summary>
        public DuelHistory History;
    }
}