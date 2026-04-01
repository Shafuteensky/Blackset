using Blackset.Data.Registries;
using Blackset.Storms;

namespace Blackset.Duel.Rules
{
    /// <summary>
    /// Конфигурация правил дуэли
    /// </summary>
    public struct DuelRulesConfiguration
    {
        /// <summary>
        /// Количество дайсов в сборке
        /// </summary>
        public int DicesInSet;
        /// <summary>
        /// Количество расходников в сборке
        /// </summary>
        public int ConsumablesInSet;

        /// <summary>
        /// Политика составления сборки дайсов
        /// </summary>
        public DiceSetPolicy DiceSetPolicy;
        /// <summary>
        /// Политика составления сборки расходников
        /// </summary>
        public ConsumableSetPolicy ConsumableSetPolicy;

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
        /// Политика генерации целевого значения
        /// </summary>
        public TargetValuePolicy TargetValuePolicy;

        /// <summary>
        /// Получить конфигурацию правил из <see cref="DuelRulesConfig"/>, настроенного в редакторе
        /// </summary>
        /// <returns>Конфигурация правил дуэли из <see cref="GameData"/></returns>
        public static DuelRulesConfiguration Default()
        {
            DuelRulesConfig config = GameData.Instance.DuelRulesConfig;

            return new DuelRulesConfiguration
            {
                DicesInSet            = config.SetComposition.DicesInSet,
                ConsumablesInSet      = config.SetComposition.ConsumablesInSet,
                DiceSetPolicy         = config.SetComposition.DiceSetPolicy,
                ConsumableSetPolicy   = config.SetComposition.ConsumableSetPolicy,

                MaxRerolls            = config.TurnLimits.MaxRerolls,
                MaxFightsPerDuel      = config.TurnLimits.MaxFightsPerDuel,
                MaxThrowsPerFight     = config.TurnLimits.MaxThrowsPerFight,

                DuelWinPolicy         = config.WinLossPolicy.DuelWinPolicy,
                FightWinPolicy        = config.WinLossPolicy.FightWinPolicy,
                FightLossPolicy       = config.WinLossPolicy.FightLossPolicy,

                DiceThrowPolicy       = config.MechanicsPolicy.DiceThrowPolicy,
                EffectsPolicy         = config.MechanicsPolicy.EffectsPolicy,
                TargetValuePolicy     = config.MechanicsPolicy.TargetValuePolicy
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