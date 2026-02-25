using Blackset.Duel.Participants;

namespace Blackset.Duel.Context
{
    /// <summary>
    /// Данные об участнике дуэли
    /// </summary>
    public struct DuelParticipantContext
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
        /// Счет боя 
        /// </summary>
        public int Score;
        /// <summary>
        /// Количество победных боев
        /// </summary>
        public int FightWon;
        /// <summary>
        /// Количество пасов за бой
        /// </summary>
        public int ThrowsPassed;
        
        /// <summary>
        /// Состояние на текущий ход
        /// </summary>
        public TurnParticipantState TurnState;
        
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