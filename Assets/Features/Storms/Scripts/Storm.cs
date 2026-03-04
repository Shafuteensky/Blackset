using Blackset.Data.Base;
using Blackset.Duel.Rules;
using UnityEngine;

namespace Blackset.Storms
{
    /// <summary>
    /// Шторм — модификатор правил конфигурации правил дуэли 
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Duel/" + nameof(Storm),
        fileName = nameof(Storm))]
    public sealed class Storm : BaseData, IDuelRulesModifier
    {
        [Header("Сборки"), Space]
        [SerializeField]
        private RuleOverride<int> dicesInSet;
        [SerializeField]
        private RuleOverride<int> consumablesInSet;
        [SerializeField]
        private RuleOverride<DiceSetPolicy> dicesSetPolicy;
        [SerializeField]
        private RuleOverride<ConsumableSetPolicy> consumablesSetPolicy;
        [SerializeField]
        private RuleOverride<int> maxRerolls;

        [Header("Дуэль"), Space]
        [SerializeField]
        private RuleOverride<DuelWinPolicy> duelWinPolicy;

        [Header("Битвы"), Space]
        [SerializeField]
        private RuleOverride<int> maxFightsPerDuel;
        [SerializeField]
        private RuleOverride<FightWinPolicy> fightWinPolicy;
        [SerializeField]
        private RuleOverride<FightLossPolicy> fightLossPolicy;

        [Header("Броски"), Space]
        [SerializeField]
        private RuleOverride<int> maxThrowsPerFight;
        [SerializeField]
        private RuleOverride<DiceThrowPolicy> diceThrowPolicy;
        [SerializeField]
        private RuleOverride<EffectsPolicy> effectsPolicy;
        
        // [Header("Целевое значение"), Space] // TODO модификация ЦЗ
        // [SerializeField]
        // private RuleOverride<TargetValueMode> targetValueMode; // enum: Random, Fixed
        // [SerializeField]
        // private RuleOverride<int> fixedTargetValue;

        /// <summary>
        /// Применить оверрайды правил дуэли к конфигурации
        /// </summary>
        /// <param name="config">Конфигурация правил для обновления</param>
        public void Apply(ref DuelRulesConfiguration config)
        {
            maxRerolls.Apply(ref config.MaxRerolls);
            dicesInSet.Apply(ref config.DicesInSet);
            consumablesInSet.Apply(ref config.ConsumablesInSet);
            consumablesSetPolicy.Apply(ref config.ConsumableSetPolicy);
            maxRerolls.Apply(ref config.MaxRerolls);
            duelWinPolicy.Apply(ref config.DuelWinPolicy);
            maxFightsPerDuel.Apply(ref config.MaxFightsPerDuel);
            fightWinPolicy.Apply(ref config.FightWinPolicy);
            fightLossPolicy.Apply(ref config.FightLossPolicy);
            maxThrowsPerFight.Apply(ref config.MaxThrowsPerFight);
            diceThrowPolicy.Apply(ref config.DiceThrowPolicy);
            effectsPolicy.Apply(ref config.EffectsPolicy);
        }
    }
}