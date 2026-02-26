using System.Collections.Generic;
using Blackset.Duel.Context;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Данные об участнике дуэли
    /// </summary>
    public struct DuelParticipantState
    {
        /// <summary>
        /// Идентификатор участника
        /// </summary>
        public string ParticipantId;
        /// <summary>
        /// Является ли участник игроком (иначе считается ботом)
        /// </summary>
        public bool IsPlayer;
        
        /// <summary>
        /// Пулы участника
        /// </summary>
        public DuelPoolsContext Pools;
        /// <summary>
        /// Сборки участника
        /// </summary>
        public DuelSetsContext Sets;
        
        /// <summary>
        /// Количество победных боев
        /// </summary>
        public int FightsWon;
        
        /// <summary>
        /// Состояние на текущий бой
        /// </summary>
        public FightParticipantState FightState;
        
        /// <summary>
        /// Уровень доверия бота
        /// </summary>
        public float TrustLevel;
        /// <summary>
        /// Уровень паники бота
        /// </summary>
        public float PanicLevel;
    }
}