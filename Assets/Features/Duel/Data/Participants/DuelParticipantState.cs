using Blackset.Duel.Pools;
using Blackset.Duel.Sets;
using Blackset.Opponents;
using Extensions.Reactive;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Данные об участнике дуэли
    /// </summary>
    public class DuelParticipantState
    {
        private const float DEFAULT_BOT_TRUST_LEVEL = 0.5f;
        
        /// <summary>
        /// Идентификатор участника
        /// </summary>
        public string ParticipantId { get; private set; }
        /// <summary>
        /// Является ли участник игроком (иначе считается ботом)
        /// </summary>
        public bool IsPlayer { get; private set; }
        /// <summary>
        /// Состояние готовности пулов
        /// </summary>
        public bool IsPoolsInited => Pools != null;
        /// <summary>
        /// Состояние готовности сборок
        /// </summary>
        public bool IsSetsInited => Sets != null;
        
        /// <summary>
        /// Пулы участника
        /// </summary>
        public DuelPoolsContext Pools { get; private set; }
        /// <summary>
        /// Сборки участника
        /// </summary>
        public DuelSetsContext Sets { get; private set; }
        
        /// <summary>
        /// Количество победных боев
        /// </summary>
        public ReactiveProperty<int> FightsWon { get; private set; } = new(0);
        /// <summary>
        /// Состояние на текущий бой
        /// </summary>
        public FightParticipantState FightState { get; private set; }
        
        /// <summary>
        /// Уровень доверия бота (от 0 до 1)
        /// </summary>
        public readonly ReactiveProperty<float> TrustLevel = new();
        /// <summary>
        /// Уровень паники бота (от 0 до 1)
        /// </summary>
        public readonly ReactiveProperty<float> PanicLevel = new();

        /// <summary>
        /// Данные об участнике дуэли
        /// </summary>
        /// <param name="participantId">Идентификатор участника</param>
        /// <param name="isPlayer">Является ли игроком</param>
        /// <param name="pools">Пулы дайсов и расходников</param>
        /// <param name="sets">Сборки дайсов и расходников</param>
        public DuelParticipantState(string participantId, bool isPlayer)
        {
            ParticipantId = participantId;
            IsPlayer = isPlayer;
            
            Sets = null;
            
            FightsWon.Value = 0;
            FightState = new FightParticipantState();
            FightState.ResetForNewFight();

            TrustLevel.Value = DEFAULT_BOT_TRUST_LEVEL;
            PanicLevel.Value = 0;
        }

        /// <summary>
        /// Отметка о победе в битве
        /// </summary>
        public void WinFight()
        {
            FightsWon.Value++;
        }

        #region Инициализация данных
        
        /// <summary>
        /// Инициализация пулов
        /// </summary>
        /// <param name="sets">Сборки дайсов и расходников</param>
        public void InitializePools(DuelPoolsContext pools)
        {
            if (IsPoolsInited) return;
            Pools = pools;
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
            TrustLevel.Value = contractOpponents.CunningLevel; // TODO обновить функцию расчета (брать от данных соперника?)
        }
        
        #endregion
    }
}