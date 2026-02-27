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
            maxRerolls.Apply(ref config.maxRerolls);
            duelWinPolicy.Apply(ref config.duelWinPolicy);
            maxFightsPerDuel.Apply(ref config.maxFightsPerDuel);
            fightWinPolicy.Apply(ref config.fightWinPolicy);
            fightLossPolicy.Apply(ref config.fightLossPolicy);
            maxThrowsPerFight.Apply(ref config.maxThrowsPerFight);
            diceThrowPolicy.Apply(ref config.diceThrowPolicy);
            effectsPolicy.Apply(ref config.effectsPolicy);
        }
    }
}