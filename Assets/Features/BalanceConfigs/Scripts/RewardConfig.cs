using System;
using Blackset.Opponents;
using UnityEngine;

namespace Blackset.Rewards
{
    /// <summary>
    /// Конфиг наград дуэли: валюта и качество невалютной награды.
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(RewardConfig),
        menuName = "Blackset/Rewards/" + nameof(RewardConfig))]
    public class RewardConfig : ScriptableObject
    {
        #region Инспектор

        [Header("Базовая награда"), Space]
        
        [SerializeField]
        protected BaseReward baseMoneyReward = new BaseReward
        {
            participation = 10,
            winBonus = 20,
            loseBonus = 5
        };

        [Header("Балансные модификаторы"), Space]
        
        [SerializeField]
        protected ScoreWeights scoreWeights = new ScoreWeights
        {
            opponentDifficultyWeight = 1f,
            unusedItemsWeight = 1f
        };

        [Header("Модификаторы от неиспользованных предметов"), Space]
        
        [SerializeField]
        protected UnusedBonus unusedBonus = new UnusedBonus
        {
            unusedDiceMoney = 0,
            unusedConsumableMoney = 0,
            unusedDiceScore = 1,
            unusedConsumableScore = 1,
            totalCap = 999
        };
        
        [Header("Наградная валюта"), Space]
        
        [SerializeField]
        protected MoneySettings moneySettings = new MoneySettings
        {
            minMultiplier = 0.8f,
            maxMultiplier = 1.4f,
            moneyCurve01 = null
        };

        [Header("Качество наградного предмета"), Space]
        
        [SerializeField]
        protected QualitySettings qualitySettings = new QualitySettings
        {
            minQuality = 0f,
            maxQuality = 1f,
            qualityCurve01 = null
        };

        #endregion

        /// <summary>
        /// Рассчитать награду валюты
        /// </summary>
        public int EvaluateMoney(
            bool isWin,
            OpponentData opponent,
            int unusedDices = 0,
            int unusedConsumables = 0)
        {
            int baseValue = baseMoneyReward.participation;

            if (isWin) baseValue += baseMoneyReward.winBonus;
            else baseValue += baseMoneyReward.loseBonus;

            baseValue += GetUnusedMoneyBonus(unusedDices, unusedConsumables);

            float score01 = EvaluateScore01(opponent, unusedDices, unusedConsumables);
            float shaped = ApplyCurve01(moneySettings.moneyCurve01, score01);

            float minM = Mathf.Max(0f, moneySettings.minMultiplier);
            float maxM = Mathf.Max(0f, moneySettings.maxMultiplier);
            float mult = Mathf.Lerp(minM, maxM, shaped);

            int value = Mathf.RoundToInt(baseValue * mult);
            if (value < 0) value = 0;

            return value;
        }

        /// <summary>
        /// Рассчитать качество награды (0..1)
        /// </summary>
        public float EvaluateQuality01(
            OpponentData opponent,
            float difficulty01,
            int unusedDices,
            int unusedConsumables)
        {
            float score01 = EvaluateScore01(opponent, unusedDices, unusedConsumables);
            float shaped = ApplyCurve01(qualitySettings.qualityCurve01, score01);

            float minQ = Mathf.Clamp01(qualitySettings.minQuality);
            float maxQ = Mathf.Clamp01(qualitySettings.maxQuality);

            return Mathf.Clamp01(Mathf.Lerp(minQ, maxQ, shaped));
        }

        #region Внутренние расчеты

        private float EvaluateScore01(
            OpponentData opponent,
            int unusedDices,
            int unusedConsumables)
        {
            // Модификатор счета от сложности соперника
            float opponentDifficultyLevel = opponent.DifficultyLevel();
            // Модификатор счета от неиспользованных предметов
            float unusedItemsScore = EvaluateUnused01(unusedDices, unusedConsumables);

            // Балансные модификаторы счета
            float balanceDifficultyModifier = Mathf.Max(0f, scoreWeights.opponentDifficultyWeight);
            float balanceUnusedModifier = Mathf.Max(0f, scoreWeights.unusedItemsWeight);

            float sumModifier = balanceDifficultyModifier + balanceUnusedModifier;
            if (sumModifier <= 0f) return 0f;

            float score = 0f;
            score += opponentDifficultyLevel * balanceDifficultyModifier;
            score += unusedItemsScore * balanceUnusedModifier;

            return Mathf.Clamp01(score / sumModifier);
        }

        private float EvaluateUnused01(int unusedDices, int unusedConsumables)
        {
            int dices = unusedDices;
            int cons = unusedConsumables;

            if (dices < 0) dices = 0;
            if (cons < 0) cons = 0;

            int total = dices + cons;
            int cap = unusedBonus.totalCap;

            if (total > cap)
            {
                int overflow = total - cap;

                int removeFromDices = Mathf.Min(dices, overflow);
                dices -= removeFromDices;
                overflow -= removeFromDices;

                if (overflow > 0)
                {
                    int removeFromCons = Mathf.Min(cons, overflow);
                    cons -= removeFromCons;
                }
            }

            int scoreRaw = 0;
            scoreRaw += dices * unusedBonus.unusedDiceScore;
            scoreRaw += cons * unusedBonus.unusedConsumableScore;

            return Mathf.Clamp01(Mathf.InverseLerp(0, 1, scoreRaw));
        }

        private int GetUnusedMoneyBonus(int unusedDices, int unusedConsumables)
        {
            int dices = unusedDices;
            int cons = unusedConsumables;

            if (dices < 0) dices = 0;
            if (cons < 0) cons = 0;

            int total = dices + cons;
            int cap = unusedBonus.totalCap;

            if (total > cap)
            {
                int overflow = total - cap;

                int removeFromDices = Mathf.Min(dices, overflow);
                dices -= removeFromDices;
                overflow -= removeFromDices;

                if (overflow > 0)
                {
                    int removeFromCons = Mathf.Min(cons, overflow);
                    cons -= removeFromCons;
                }
            }

            int value = 0;
            value += dices * unusedBonus.unusedDiceMoney;
            value += cons * unusedBonus.unusedConsumableMoney;

            if (value < 0) value = 0;
            return value;
        }

        private float ApplyCurve01(AnimationCurve curve, float t)
        {
            t = Mathf.Clamp01(t);
            if (curve == null) return t;
            return Mathf.Clamp01(curve.Evaluate(t));
        }

        #endregion
        
        #region Вспомогательные типы

        [Serializable]
        public struct BaseReward
        {
            [Min(0)] public int participation;
            [Min(0)] public int winBonus;
            [Min(0)] public int loseBonus;
        }

        [Serializable]
        public struct ScoreWeights
        {
            [Min(0f)] public float opponentDifficultyWeight;
            [Min(0f)] public float unusedItemsWeight;
        }

        [Serializable]
        public struct UnusedBonus
        {
            [Min(0)] public int unusedDiceMoney;
            [Min(0)] public int unusedConsumableMoney;

            [Min(0)] public int unusedDiceScore;
            [Min(0)] public int unusedConsumableScore;

            [Min(0)] public int totalCap;
        }

        [Serializable]
        public struct MoneySettings
        {
            [Min(0f)] public float minMultiplier;
            [Min(0f)] public float maxMultiplier;

            public AnimationCurve moneyCurve01;
        }

        [Serializable]
        public struct QualitySettings
        {
            [Min(0f)] public float minQuality;
            [Min(0f)] public float maxQuality;

            public AnimationCurve qualityCurve01;
        }

        #endregion
    }
}