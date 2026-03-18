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
        };
        
        [Header("Наградная валюта"), Space]
        
        [SerializeField]
        protected MoneySettings moneySettings = new MoneySettings
        {
            minMultiplier = 0.8f,
            maxMultiplier = 1.4f,
            moneyCurve01 = null
        };
        
        [Space]
        [SerializeField]
        protected BaseRewardMultipliers rewardMultipliers = new BaseRewardMultipliers
        {
            winMultiplier = 1f,
            loseMultiplier = 0.5f
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
            OpponentData opponent)
        {
            int baseValue = baseMoneyReward.participation;

            if (isWin) baseValue = (int)Math.Round((baseValue * rewardMultipliers.winMultiplier) + baseMoneyReward.winBonus);
            else baseValue = (int)Math.Round((baseValue * rewardMultipliers.loseMultiplier) + baseMoneyReward.loseBonus) ;

            float score01 = EvaluateScore01(opponent);
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
        public float EvaluateQuality01(OpponentData opponent)
        {
            float score01 = EvaluateScore01(opponent);
            float shaped = ApplyCurve01(qualitySettings.qualityCurve01, score01);

            float minQ = Mathf.Clamp01(qualitySettings.minQuality);
            float maxQ = Mathf.Clamp01(qualitySettings.maxQuality);

            return Mathf.Clamp01(Mathf.Lerp(minQ, maxQ, shaped));
        }

        #region Внутренние расчеты

        private float EvaluateScore01(OpponentData opponent)
        {
            // Модификатор счета от сложности соперника
            float opponentDifficultyLevel = opponent.DifficultyLevel();

            // Балансные модификаторы счета
            float balanceDifficultyModifier = Mathf.Max(0f, scoreWeights.opponentDifficultyWeight);

            float sumModifier = balanceDifficultyModifier;
            if (sumModifier <= 0f) return 0f;

            float score = 0f;
            score += opponentDifficultyLevel * balanceDifficultyModifier;

            return Mathf.Clamp01(score / sumModifier);
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
        public struct BaseRewardMultipliers
        {
            [Range(0f, 2f)] public float winMultiplier;
            [Range(0f, 2f)] public float loseMultiplier;
        }

        [Serializable]
        public struct ScoreWeights
        {
            [Min(0f)] public float opponentDifficultyWeight;
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