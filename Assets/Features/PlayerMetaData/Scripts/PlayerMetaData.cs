using Unity.VisualScripting;
using UnityEngine;

namespace Blackset.Player
{
    /// <summary>
    /// Структура мета-данных игрока
    /// </summary>
    public class PlayerMetaData
    {
        #region Константы

        // Опыт и уровень
        private const int EXP_A = 50; // Ускорение роста (квадратичная часть) — изменяет “жёсткость лейта”
        private const int EXP_B = 150; // Базовая линейная “цена уровня” — изменяет “скорость старта”

        // Бюджет дайсов
        private const int DEFAULT_START_BUDGET = 6; // Дефолтный начальный бюджет
        private const int BUDGET_PER_LEVEL = 1; // Прибавка бюджета на уровень
        private const int BUDGET_MILESTONE_LEVEL_STEP = 5; // Интервал уровней для получения бонуса
        private const int BUDGET_MILESTONE_BONUS = 3; // Размер бонуса
        
        #endregion     
        
        /// <summary>
        /// Количество софт-валюты
        /// </summary>
        public int Money => money;
        /// <summary>
        /// Суммарный опыт
        /// </summary>
        public int SumExperience => sumExperience;
        /// <summary>
        /// Счетчик уровней игрока
        /// </summary>
        public int PlayerLevelCounter => playerLevelCounter;

        [Serialize]
        private int money;
        [Serialize]
        private int sumExperience;
        [Serialize]
        private int playerLevelCounter;

        #region Money

        /// <summary>
        /// Добавить деньги
        /// </summary>
        /// <param name="amount"></param>
        public void AddMoney(int amount)
        {
            if (amount <= 0) return;
            money += amount;
        }
        
        /// <summary>
        /// Убавить деньги
        /// </summary>
        /// <param name="amount"></param>
        public void RemoveMoney(int amount)
        {
            if (amount <= 0) return;
            money -= amount;
        }
        
        #endregion
        
        #region Experience

        /// <summary>
        /// Добавить очки опыта
        /// </summary>
        /// <param name="amount"></param>
        public void AddExperience(int amount)
        {
            if (amount <= 0) return;
            sumExperience += amount;
        }

        /// <summary>
        /// Добавить очки опыта
        /// </summary>
        /// <param name="amount"></param>
        public void SetLevelCounter(int lvl)
        {
            if (lvl <= 0) return;
            playerLevelCounter = lvl;
        }
        
        /// <summary>
        /// Рассчитать суммарный требуемый опыт на определенный уровень
        /// </summary>
        /// <param name="level">Уровень</param>
        /// <returns>Количество опыта на указанный уровень</returns>
        public int GetTotalExpForLevel(int level)
        {
            if (level <= 1) return 0;

            int n = level - 1;

            // Формула суммарного опыта на уровень:
            // TotalExp(L) = EXP_A*(L-1)^2 + EXP_B*(L-1)
            return (EXP_A * n * n) + (EXP_B * n);
        }

        /// <summary>
        /// Текущий уровень игрока
        /// </summary>
        public int GetPlayerLvl()
        {
            if (sumExperience <= 0) return 1;

            // Обратная формула к TotalExp(L):
            // EXP_A*n^2 + EXP_B*n - SumExperience = 0, где n = (L-1)
            // n = floor( (-EXP_B + sqrt(EXP_B^2 + 4*EXP_A*SumExperience)) / (2*EXP_A) )
            float disc = (EXP_B * EXP_B) + (4f * EXP_A * sumExperience);
            float sqrt = Mathf.Sqrt(disc);

            int n = Mathf.FloorToInt((-EXP_B + sqrt) / (2f * EXP_A));
            int lvl = n + 1;

            if (lvl < 1) lvl = 1;
            return lvl;
        }

        /// <summary>
        /// Сколько опыта набрано на текущем уровне
        /// </summary>
        public int GetThisLevelExp()
        {
            int lvl = GetPlayerLvl();
            int start = GetTotalExpForLevel(lvl);
            int value = sumExperience - start;
            if (value < 0) value = 0;
            return value;
        }

        /// <summary>
        /// Сколько опыта осталось получить до достижения нового уровня от актуального
        /// </summary>
        public int GetExpToNextLevel()
        {
            int lvl = GetPlayerLvl();
            int next = GetTotalExpForLevel(lvl + 1);
            int value = next - sumExperience;
            if (value < 0) value = 0;
            return value;
        }

        /// <summary>
        /// Требуемое суммарное кличество опыта на текущем уровне
        /// </summary>
        public int GetThisLevelRequiredExp()
        {
            int lvl = GetPlayerLvl();
            int start = GetTotalExpForLevel(lvl);
            int next = GetTotalExpForLevel(lvl + 1);
            int value = next - start;
            if (value < 1) value = 1;
            return value;
        }

        /// <summary>
        /// Прогресс получения опыта на текущем уровне
        /// </summary>
        /// <returns>Значение от 0 до 1</returns>
        public float GetProgressToNextLevel()
        {
            int need = GetThisLevelRequiredExp();
            int have = GetThisLevelExp();

            if (need <= 0) return 0f;

            float v = (float)have / need;
            return Mathf.Clamp01(v);
        }
        
        #endregion

        #region Dice Budget

        /// <summary>
        /// Максимальный актуальный бюджет сборки дайсов
        /// </summary>
        /// <returns></returns>
        public int GetActualMaxDiceBudget()
        {
            int lvl = GetPlayerLvl();

            // Формула бюджета:
            // Budget = DEFAULT_START_BUDGET
            //        + BUDGET_PER_LEVEL*(L-1)
            //        + BUDGET_MILESTONE_BONUS*floor((L-1)/BUDGET_MILESTONE_LEVEL_STEP)
            int perLevel = BUDGET_PER_LEVEL * (lvl - 1);
            int milestoneBonus = BUDGET_MILESTONE_BONUS * ((lvl - 1) / BUDGET_MILESTONE_LEVEL_STEP);

            int value = DEFAULT_START_BUDGET + perLevel + milestoneBonus;
            if (value < 0) value = 0;

            return value;
        }
        
        #endregion
    }
}
