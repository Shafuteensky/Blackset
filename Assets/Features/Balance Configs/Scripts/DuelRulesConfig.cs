using System;
using System.Collections.Generic;
using Blackset.BalanceConfigs;
using Blackset.Data.Items.Types;
using UnityEngine;

namespace Blackset.Duel.Rules
{
    /// <summary>
    /// Конфигурация правил дуэли
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(DuelRulesConfig),
        menuName = "Blackset/Duel/" + nameof(DuelRulesConfig))]
    public class DuelRulesConfig : BaseBalanceConfig
    {
        #region Инспектор

        [Header("Счета участников дуэли"), Space]
        [SerializeField]
        public ScoresSettings Scores = new()
        {
            MinDuelScore = 0,
            MinFightScore = 0
        };

        [Header("Состав сборки"), Space]
        [SerializeField]
        public SetCompositionSettings SetComposition = new()
        {
            DicesInSet = 6,
            ConsumablesInSet = 3,
            DiceSetPolicy = DiceSetPolicy.OneInType,
            ConsumableSetPolicy = ConsumableSetPolicy.RandomLimited
        };

        [Header("Ограничения ходов и битв"), Space]
        [SerializeField]
        public TurnLimitSettings TurnLimits = new()
        {
            MaxRerolls = 1,
            MaxFightsPerDuel = 5,
            MaxThrowsPerFight = 6
        };

        [Header("Политики победы и проигрыша"), Space]
        [SerializeField]
        public WinLossPolicySettings WinLossPolicy = new()
        {
            DuelWinPolicy = DuelWinPolicy.WinMostFights,
            FightWinPolicy = FightWinPolicy.ExactOrClosest,
            FightLossPolicy = FightLossPolicy.LessOrBust
        };

        [Header("Политики бросков и эффектов"), Space]
        [SerializeField]
        public MechanicsPolicySettings MechanicsPolicy = new()
        {
            DiceThrowPolicy = DiceThrowPolicy.Once,
            EffectsPolicy = EffectsPolicy.Both,
        };

        [Header("Настройки целевого значения"), Space]
        [SerializeField]
        public TargetValueSettings TargetValue = new()
        {
            TargetValuePolicy = TargetValuePolicy.RandomSet,
            DefaultDicesForTargetGeneration = new List<DiceType>()
        };

        #endregion

        #region Вспомогательные типы

        [Serializable]
        public struct ScoresSettings
        {
            public int MinFightScore;
            public int MinDuelScore;
        }

        [Serializable]
        public struct SetCompositionSettings
        {
            [Min(1)] public int DicesInSet;
            [Min(0)] public int ConsumablesInSet;
            public DiceSetPolicy DiceSetPolicy;
            public ConsumableSetPolicy ConsumableSetPolicy;
        }

        [Serializable]
        public struct TurnLimitSettings
        {
            [Min(0)] public int MaxRerolls;
            [Min(1)] public int MaxFightsPerDuel;
            [Min(1)] public int MaxThrowsPerFight;
        }

        [Serializable]
        public struct WinLossPolicySettings
        {
            public DuelWinPolicy DuelWinPolicy;
            public FightWinPolicy FightWinPolicy;
            public FightLossPolicy FightLossPolicy;
        }

        [Serializable]
        public struct MechanicsPolicySettings
        {
            public DiceThrowPolicy DiceThrowPolicy;
            public EffectsPolicy EffectsPolicy;
        }

        [Serializable]
        public struct TargetValueSettings
        {
            public TargetValuePolicy TargetValuePolicy;
            public List<DiceType> DefaultDicesForTargetGeneration;
        }

        #endregion
    }
}