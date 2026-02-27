using Blackset.Duel.Context;
using Blackset.Opponents;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Данные об участнике дуэли
    /// </summary>
    public struct DuelParticipantState
    {
        private const float DEFAULT_BOT_TRUST_LEVEL = 0.5f;
        
        /// <summary>
        /// Идентификатор участника
        /// </summary>
        public string ParticipantId { get; }
        /// <summary>
        /// Является ли участник игроком (иначе считается ботом)
        /// </summary>
        public bool IsPlayer { get; }
        /// <summary>
        /// Состояние готовности сборок
        /// </summary>
        public bool IsSetsInited => Sets != null;
        
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
        public FightParticipantState FightState { get; }
        
        /// <summary>
        /// Уровень доверия бота (от 0 до 1)
        /// </summary>
        public float TrustLevel;
        /// <summary>
        /// Уровень паники бота (от 0 до 1)
        /// </summary>
        public float PanicLevel;

        /// <summary>
        /// Данные об участнике дуэли
        /// </summary>
        /// <param name="participantId">Идентификатор участника</param>
        /// <param name="isPlayer">Является ли игроком</param>
        /// <param name="pools">Пулы дайсов и расходников</param>
        /// <param name="sets">Сборки дайсов и расходников</param>
        public DuelParticipantState(string participantId, bool isPlayer, DuelPoolsContext pools)
        {
            ParticipantId = participantId;
            IsPlayer = isPlayer;
            
            Pools = pools;
            Sets = null;
            
            FightsWon = 0;
            FightState = new FightParticipantState();
            FightState.ResetForNewFight();

            TrustLevel = DEFAULT_BOT_TRUST_LEVEL;
            PanicLevel = 0;
        }

        /// <summary>
        /// Инициализация сборок
        /// </summary>
        /// <param name="sets">Сборки дайсов и расходников</param>
        public void InitializeSets(DuelSetsContext sets)
        {
            if (IsSetsInited) return;
            Sets = sets;
        }

        /// <summary>
        /// Инициализация данных участника-бота
        /// </summary>
        /// <param name="contractOpponents"></param>
        public void InitializeBotDuelState(OpponentData contractOpponents)
        {
            TrustLevel = contractOpponents.CunningLevel; // TODO обновить функцию расчета (брать от данных соперника?)
        }
    }
}