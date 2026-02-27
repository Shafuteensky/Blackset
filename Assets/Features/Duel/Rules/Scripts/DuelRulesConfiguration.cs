using Blackset.Storms;

namespace Blackset.Duel.Rules
{
    /// <summary>
    /// Конфигурация правил дуэли
    /// </summary>
    public struct DuelRulesConfiguration
    {
        #region Стандартные правила игры
        
        private const int DEFAULT_MAX_REROLLS = 1;
        private const int DEFAULT_MAX_FIGHTS_PER_DUEL = 3;
        private const int DEFAULT_MAX_THROWS_PER_FIGHT = 6;
        
        private const DuelWinPolicy DEFAULT_DUEL_WIN_POLICY = DuelWinPolicy.WinMostFights;
        private const FightWinPolicy DEFAULT_FIGHT_WIN_POLICY = FightWinPolicy.ExactOrClosest;
        private const FightLossPolicy DEFAULT_FIGHT_LOSS_POLICY = FightLossPolicy.LessOrBust;
        private const DiceThrowPolicy DEFAULT_DICE_THROW_POLICY = DiceThrowPolicy.Once;
        private const EffectsPolicy DEFAULT_EFFECTS_POLICY = EffectsPolicy.Both;
        
        #endregion
        
        /// <summary>
        /// Максимум рероллов на класс предметов (дайсы, расходники)
        /// </summary>
        public int MaxRerolls;
        /// <summary>
        /// Максимальное количество битв за дуэль
        /// </summary>
        public int MaxFightsPerDuel;
        /// <summary>
        /// Максимальное количество ходов (бросков дайсов) за битву
        /// </summary>
        public int MaxThrowsPerFight;
        
        /// <summary>
        /// Политика победы в дуэли
        /// </summary>
        public DuelWinPolicy DuelWinPolicy;
        /// <summary>
        /// Политика победы в битве в составе дуэли
        /// </summary>
        public FightWinPolicy FightWinPolicy;
        /// <summary>
        /// Политика проигрыша в битве в составе дуэли
        /// </summary>
        public FightLossPolicy FightLossPolicy;
        /// <summary>
        /// Политика броска дайса в течении одной битвы
        /// </summary>
        public DiceThrowPolicy DiceThrowPolicy;
        /// <summary>
        /// Политика применения эффектов
        /// </summary>
        public EffectsPolicy EffectsPolicy;

        /// <summary>
        /// Получить дефолтную конфигурацию правил
        /// </summary>
        /// <returns>Конфигурация правил дуэли со стандартными установками</returns>
        public static DuelRulesConfiguration Default()
        {
            return new DuelRulesConfiguration
            {
                MaxRerolls = DEFAULT_MAX_REROLLS,
                MaxFightsPerDuel = DEFAULT_MAX_FIGHTS_PER_DUEL,
                MaxThrowsPerFight = DEFAULT_MAX_THROWS_PER_FIGHT,
                
                DuelWinPolicy = DEFAULT_DUEL_WIN_POLICY,
                FightWinPolicy = DEFAULT_FIGHT_WIN_POLICY,
                FightLossPolicy = DEFAULT_FIGHT_LOSS_POLICY,
                DiceThrowPolicy = DEFAULT_DICE_THROW_POLICY,
                EffectsPolicy = DEFAULT_EFFECTS_POLICY
            };
        }
        
        /// <summary>
        /// Применить модификатор правил
        /// </summary>
        public DuelRulesConfiguration ApplyModifier(IDuelRulesModifier modifier)
        {
            DuelRulesConfiguration copy = this;
            modifier.Apply(ref copy);
            return copy;
        }
    }
}