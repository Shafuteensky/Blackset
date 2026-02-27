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
        public int maxRerolls;
        /// <summary>
        /// Максимальное количество битв за дуэль
        /// </summary>
        public int maxFightsPerDuel;
        /// <summary>
        /// Максимальное количество ходов (бросков дайсов) за битву
        /// </summary>
        public int maxThrowsPerFight;
        
        /// <summary>
        /// Политика победы в дуэли
        /// </summary>
        public DuelWinPolicy duelWinPolicy;
        /// <summary>
        /// Политика победы в битве в составе дуэли
        /// </summary>
        public FightWinPolicy fightWinPolicy;
        /// <summary>
        /// Политика проигрыша в битве в составе дуэли
        /// </summary>
        public FightLossPolicy fightLossPolicy;
        /// <summary>
        /// Политика броска дайса в течении одной битвы
        /// </summary>
        public DiceThrowPolicy diceThrowPolicy;
        /// <summary>
        /// Политика применения эффектов
        /// </summary>
        public EffectsPolicy effectsPolicy;

        /// <summary>
        /// Получить дефолтную конфигурацию правил
        /// </summary>
        /// <returns>Конфигурация правил дуэли со стандартными установками</returns>
        public static DuelRulesConfiguration Default()
        {
            return new DuelRulesConfiguration
            {
                maxRerolls = DEFAULT_MAX_REROLLS,
                maxFightsPerDuel = DEFAULT_MAX_FIGHTS_PER_DUEL,
                maxThrowsPerFight = DEFAULT_MAX_THROWS_PER_FIGHT,
                
                duelWinPolicy = DEFAULT_DUEL_WIN_POLICY,
                fightWinPolicy = DEFAULT_FIGHT_WIN_POLICY,
                fightLossPolicy = DEFAULT_FIGHT_LOSS_POLICY,
                diceThrowPolicy = DEFAULT_DICE_THROW_POLICY,
                effectsPolicy = DEFAULT_EFFECTS_POLICY
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