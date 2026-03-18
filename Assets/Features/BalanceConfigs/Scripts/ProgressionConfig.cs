using System;
using System.Collections.Generic;
using Blackset.Opponents;
using UnityEngine;

namespace Features.Progression
{
    /// <summary>
    /// Конфигурация прогресса игрока (балансные точки отсчета): XP, уровни, бюджет сборки, гейты лиг и награды
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ProgressionConfig),
        menuName = "Blackset/Player/" + nameof(ProgressionConfig))]
    public sealed class ProgressionConfig : ScriptableObject
    {
        #region Дефолтные значения

        // Опыт и уровень
        private const int EXP_A = 50; // Ускорение роста (квадратичная часть) — изменяет “жёсткость лейта”
        private const int EXP_B = 150; // Базовая линейная “цена уровня” — изменяет “скорость старта”

        // Бюджет дайсов
        private const int DEFAULT_START_BUDGET = 6; // Дефолтный начальный бюджет
        private const int BUDGET_PER_LEVEL = 1; // Прибавка бюджета на уровень
        private const int BUDGET_MILESTONE_LEVEL_STEP = 5; // Интервал уровней для получения бонуса
        private const int BUDGET_MILESTONE_BONUS = 3; // Размер бонуса
        
        #endregion   
        
        #region Инспектор

        [Header("Опыт"), Space]
        
        [SerializeField]
        private int levelCap;
        
        [Space]
        [SerializeField]
        private ExperienceFormulaAB experienceFormula = new ExperienceFormulaAB
        {
            expA = EXP_A,
            expB = EXP_B
        };

        [Header("Бюджет сборки дайсов"), Space]
        
        [SerializeField]
        private DiceBudgetFormula diceBudgetFormula = new DiceBudgetFormula
        {
            startBudget = DEFAULT_START_BUDGET,
            budgetPerLevel = BUDGET_PER_LEVEL,
            milestoneLevelStep = BUDGET_MILESTONE_LEVEL_STEP,
            milestoneBonus = BUDGET_MILESTONE_BONUS
        };

        [Header("Награды"), Space]
        
        [SerializeField]
        private ExperienceRewards experienceRewards = new ExperienceRewards
        {
            participationBonus = 10,
            winModifier = 1f,
            loseModifier = 0.75f
        };
        
        [Space]
        [SerializeField]
        private RewardsMultipliers rewardsMultipliers = new RewardsMultipliers
        {
            leagueMultiplier = 1f,
            modeMultiplier = 1f
        };
        
        [Space]
        [SerializeField]
        private OpponentDifficultyMultipliers opponentDifficultyMultipliers = new OpponentDifficultyMultipliers
        {
            buildValueWeight = 1f,
            masteryWeight = 1f,
            cunningWeight = 1f,
            minMultiplier = 0.8f,
            maxMultiplier = 1.4f,
            sumModifierWeight = 1f 
        };

        [Header("Лиги (WIP)"), Space]
        
        [SerializeField]
        private List<LeagueGateEntry> leagueGates = new List<LeagueGateEntry>();

        #endregion

        #region Опыт игрока

        /// <summary>
        /// Рассчитать суммарный требуемый опыт на указанный уровень
        /// </summary>
        public int GetTotalExpForLevel(int level)
        {
            if (level <= 1) return 0;
            int clamped = ClampLevel(level);

            return GetTotalExpForLevelFromFormula(clamped);
        }

        /// <summary>
        /// Рассчитать уровень игрока по суммарному опыту
        /// </summary>
        public int GetLevelByTotalExp(int sumExperience)
        {
            if (sumExperience <= 0) return 1;

            return GetLevelByTotalExpFromFormula(sumExperience);
        }

        private int GetTotalExpForLevelFromFormula(int level)
        {
            int n = level - 1;
            int a = experienceFormula.expA;
            int b = experienceFormula.expB;

            return (a * n * n) + (b * n);
        }

        private int GetLevelByTotalExpFromFormula(int sumExperience)
        {
            int a = experienceFormula.expA;
            int b = experienceFormula.expB;

            if (a <= 0)
            {
                if (b <= 0) return 1;

                int linear = (sumExperience / b) + 1;
                return ClampLevel(linear);
            }

            float disc = (b * b) + (4f * a * sumExperience);
            float sqrt = Mathf.Sqrt(disc);

            int n = Mathf.FloorToInt((-b + sqrt) / (2f * a));
            int lvl = n + 1;

            if (lvl < 1) lvl = 1;
            return ClampLevel(lvl);
        }

        #endregion

        #region Бюджет сборки дайсов

        /// <summary>
        /// Максимальный бюджет сборки дайсов на указанном уровне
        /// </summary>
        public int GetMaxDiceBudgetForLevel(int level)
        {
            int clamped = ClampLevel(level);
            int value = GetMaxDiceBudgetByFormula(clamped);
            if (value < 0) value = 0;

            return value;
        }

        private int GetMaxDiceBudgetByFormula(int level)
        {
            int start = diceBudgetFormula.startBudget;
            int perLevel = diceBudgetFormula.budgetPerLevel * (level - 1);
            int step = diceBudgetFormula.milestoneLevelStep;
            int milestone = diceBudgetFormula.milestoneBonus * ((level - 1) / step);

            return start + perLevel + milestone;
        }

        #endregion

        #region Награды за дуэли

        /// <summary>
        /// Рассчитать награду опыта за бой
        /// </summary>
        public int GetBattleExperienceReward(
            bool isWin,
            OpponentData opponent,
            int duelScore,
            float league = 1f,
            float mode = 1f)
        {
            // Базовый бонус за участие
            int baseReward = duelScore + experienceRewards.participationBonus;

            // Модификатор победа/проигрышь
            float modifier = 1f;
            if (isWin) 
                modifier = experienceRewards.winModifier;
            else 
                modifier = experienceRewards.loseModifier;
            baseReward = (int)Math.Round(baseReward * modifier);

            // Модификатор от режима 
            // TODO Обновить для лиги и т.д.
            float mult = 1f;
            mult *= Mathf.Max(0f, rewardsMultipliers.leagueMultiplier) * Mathf.Max(0f, league);
            mult *= Mathf.Max(0f, rewardsMultipliers.modeMultiplier) * Mathf.Max(0f, mode);

            // Модификатор от сложности соперника
            mult *= GetOpponentDifficultyMultiplier(opponent) * opponentDifficultyMultipliers.sumModifierWeight;

            int value = Mathf.RoundToInt(baseReward * mult);
            if (value < 0) value = 0;

            return value;
        }
        
        #endregion

        #region Лиги

        /// <summary>
        /// Проверить доступность лиги по уровню и суммарному опыту
        /// </summary>
        public bool IsLeagueAvailable(string leagueId, int playerLevel, int sumExperience)
        {
            if (leagueGates == null || leagueGates.Count == 0) return true;

            foreach (var entry in leagueGates)
            {
                if (entry.leagueId == leagueId)
                {
                    if (playerLevel < entry.requiredLevel) return false;
                    if (sumExperience < entry.requiredTotalExp) return false;
                    return true;
                }
            }

            return true;
        }

        #endregion

        #region Хелперы

        private int ClampLevel(int level)
        {
            if (level < 1) level = 1;

            if (levelCap > 0 && level > levelCap)
            {
                level = levelCap;
            }

            return level;
        }
        private float GetOpponentDifficultyMultiplier(OpponentData opponent)
        {
            if (opponent == null) return 1f;

            float score = 0f;
            score += opponent.BuildValue * opponentDifficultyMultipliers.buildValueWeight;
            score += opponent.MasteryLevel * opponentDifficultyMultipliers.masteryWeight;
            score += opponent.CunningLevel * opponentDifficultyMultipliers.cunningWeight;

            float wSum = 0f;
            wSum += opponentDifficultyMultipliers.buildValueWeight;
            wSum += opponentDifficultyMultipliers.masteryWeight;
            wSum += opponentDifficultyMultipliers.cunningWeight;

            if (wSum > 0f) score /= wSum;

            float t = Mathf.Clamp01(score);
            float minM = Mathf.Max(0f, opponentDifficultyMultipliers.minMultiplier);
            float maxM = Mathf.Max(0f, opponentDifficultyMultipliers.maxMultiplier);

            return Mathf.Lerp(minM, maxM, t);
        }
        
        #endregion
        
        #region Вспомогательные типы
        
        [Serializable]
        public struct OpponentDifficultyMultipliers
        {
            [Range(0f, 2f)]
            public float buildValueWeight;
            [Range(0f, 2f)]
            public float masteryWeight;
            [Range(0f, 2f)]
            public float cunningWeight;

            [Range(0f, 2f)]
            public float minMultiplier;
            [Range(0f, 2f)]
            public float maxMultiplier;
            
            [Range(0f, 2f)]
            public float sumModifierWeight;
        }

        [Serializable]
        public struct ExperienceFormulaAB
        {
            [Min(0)]
            public int expA;
            [Min(0)]
            public int expB;
        }

        [Serializable]
        public struct LevelTotalExpEntry
        {
            [Min(1)]
            public int level;
            [Min(0)]
            public int totalExpRequired;
        }

        [Serializable]
        public struct DiceBudgetFormula
        {
            [Min(0)]
            public int startBudget;
            [Min(0)]
            public int budgetPerLevel;
            [Min(1)]
            public int milestoneLevelStep;
            [Min(0)]
            public int milestoneBonus;
        }

        [Serializable]
        public struct ExperienceRewards
        {
            [Min(0)]
            public int participationBonus;
            [Range(0f, 2f)]
            public float winModifier;
            [Range(0f, 2f)]
            public float loseModifier;
        }

        [Serializable]
        public struct RewardsMultipliers
        {
            [Range(0f, 2f)]
            public float leagueMultiplier;
            [Range(0f, 2f)]
            public float modeMultiplier;
        }

        [Serializable]
        public struct LeagueGateEntry
        {
            public string leagueId;
            [Min(1)]
            public int requiredLevel;
            [Min(0)]
            public int requiredTotalExp;
            [Min(0)]
            public int recommendedDiceBudget;
        }

        #endregion
    }
}