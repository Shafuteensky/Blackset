using System;
using UnityEngine;

namespace Blackset.DuelScore
{
    /// <summary>
    /// Конфигурация баланса очков дуэли
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(DuelScoreConfig),
        menuName = "Blackset/Duel/" + nameof(DuelScoreConfig))]
    public class DuelScoreConfig : ScriptableObject
    {
        #region Инспектор

        [Header("Очки за неиспользованные предметы"), Space]

        [SerializeField]
        public UnusedItemsScore UnusedItems = new()
        {
            unusedDiceScore = 1,
            unusedConsumableScore = 1
        };

        [Header("Очки за объявления дайса"), Space]

        [SerializeField]
        public DeclarationScore Declaration = new()
        {
            bluffDeclarationScore = 2,
            honestDeclarationScore = 1,
            successfulBluffScore = 2,
            caughtEnemyBluffScore = 2
        };

        [Header("Очки за исход и темп дуэли"), Space]

        [SerializeField]
        public BattleResultScore BattleResult = new()
        {
            battleWinScore = 3,
            quickWinScore = 2,
            comebackWinScore = 3
        };

        [Header("Очки за точность и удачу"), Space]

        [SerializeField]
        public PrecisionScore Precision = new()
        {
            diceCriticalScore = 1,
            exactTargetZoneHitScore = 2
        };

        #endregion

        #region Вспомогательные типы

        [Serializable]
        public struct UnusedItemsScore
        {
            [Min(0)] public int unusedDiceScore;
            [Min(0)] public int unusedConsumableScore;
        }

        [Serializable]
        public struct DeclarationScore
        {
            [Min(0)] public int bluffDeclarationScore;
            [Min(0)] public int honestDeclarationScore;
            [Min(0)] public int successfulBluffScore;
            [Min(0)] public int caughtEnemyBluffScore;
        }

        [Serializable]
        public struct BattleResultScore
        {
            [Min(0)] public int battleWinScore;
            [Min(0)] public int quickWinScore;
            [Min(0)] public int comebackWinScore;
        }

        [Serializable]
        public struct PrecisionScore
        {
            [Min(0)] public int diceCriticalScore;
            [Min(0)] public int exactTargetZoneHitScore;
        }

        #endregion
    }
}