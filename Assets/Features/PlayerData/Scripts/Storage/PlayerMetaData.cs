using System;
using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Registries;
using UnityEngine;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Extensions.Log;
using Features.Progression;

namespace Blackset.Player
{
    /// <summary>
    /// Структура мета-данных игрока
    /// </summary>
    [Serializable]
    public class PlayerMetaData
    {
        /// <summary>
        /// Количество софт-валюты
        /// </summary>
        public int Money { get; private set; }
        /// <summary>
        /// Суммарный опыт
        /// </summary>
        public int SumExperience { get; private set; }
        /// <summary>
        /// Счетчик уровней игрока
        /// </summary>
        public int PlayerLevelCounter { get; private set; }

        #region Money

        /// <summary>
        /// Добавить деньги
        /// </summary>
        /// <param name="amount"></param>
        public void AddMoney(int amount)
        {
            if (amount <= 0) return;
            Money += amount;
        }

        /// <summary>
        /// Убавить деньги
        /// </summary>
        /// <param name="amount"></param>
        public void RemoveMoney(int amount)
        {
            if (amount <= 0) return;
            Money -= amount;
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
            SumExperience += amount;
        }

        /// <summary>
        /// Добавить очки опыта
        /// </summary>
        /// <param name="amount"></param>
        public void SetLevelCounter(int lvl)
        {
            if (lvl <= 0) return;
            PlayerLevelCounter = lvl;
        }

        /// <summary>
        /// Рассчитать суммарный требуемый опыт на определенный уровень
        /// </summary>
        /// <param name="level">Уровень</param>
        /// <returns>Количество опыта на указанный уровень</returns>
        public int GetTotalExpForLevel(int level)
        {
            if (level <= 1) return 0;

            ProgressionConfig config = GameData.Instance.ProgressionConfig;
            if (config == null)
            {
                ServiceDebug.LogError("GameData.ProgressionConfig не задан");
                return 0;
            }

            return config.GetTotalExpForLevel(level);
        }

        /// <summary>
        /// Текущий уровень игрока
        /// </summary>
        public int GetPlayerLvl()
        {
            ProgressionConfig config = GameData.Instance.ProgressionConfig;
            if (config == null)
            {
                if (SumExperience <= 0) return 1;

                ServiceDebug.LogError("GameData.ProgressionConfig не задан");
                return 1;
            }

            return config.GetLevelByTotalExp(SumExperience);
        }

        /// <summary>
        /// Сколько опыта набрано на текущем уровне
        /// </summary>
        public int GetThisLevelExp()
        {
            int lvl = GetPlayerLvl();

            ProgressionConfig config = GameData.Instance.ProgressionConfig;
            if (config == null) return 0;

            int start = config.GetTotalExpForLevel(lvl);
            int value = SumExperience - start;
            if (value < 0) value = 0;
            return value;
        }

        /// <summary>
        /// Сколько опыта осталось получить до достижения нового уровня от актуального
        /// </summary>
        public int GetExpToNextLevel()
        {
            int lvl = GetPlayerLvl();

            ProgressionConfig config = GameData.Instance.ProgressionConfig;
            if (config == null) return 0;

            int next = config.GetTotalExpForLevel(lvl + 1);
            int value = next - SumExperience;
            if (value < 0) value = 0;
            return value;
        }

        /// <summary>
        /// Требуемое суммарное кличество опыта на текущем уровне
        /// </summary>
        public int GetThisLevelRequiredExp()
        {
            int lvl = GetPlayerLvl();

            ProgressionConfig config = GameData.Instance.ProgressionConfig;
            if (config == null) return 1;

            int start = config.GetTotalExpForLevel(lvl);
            int next = config.GetTotalExpForLevel(lvl + 1);
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
        /// <returns>Целочисленное значение максимального бюджета сборки дайсов игрока</returns>
        public int GetActualMaxDiceBudget()
        {
            int lvl = GetPlayerLvl();

            ProgressionConfig config = GameData.Instance.ProgressionConfig;
            if (config == null)
            {
                ServiceDebug.LogError("GameData.ProgressionConfig не задан");
                return 0;
            }

            return config.GetMaxDiceBudgetForLevel(lvl);
        }

        /// <summary>
        /// Текущий использованный бюджет сборки дайсов
        /// </summary>
        /// <returns>Целочисленное значение максимального бюджета сборки дайсов игрока</returns>
        public int GetTotalUsedDiceBudget(List<Inventory> pools)
        {
            if (pools == null || pools.Count == 0)
            {
                ServiceDebug.LogError("Переданы невалидные пулы");
                return 0;
            }

            int totalUsedBudget = 0;
            foreach (Inventory pool in pools)
            {
                totalUsedBudget += GetUsedDiceBudgetByPool(pool);
            }

            return totalUsedBudget;
        }

        /// <summary>
        /// Текущий использованный бюджет сборки дайсов определенного номинала
        /// </summary>
        /// <returns>Целочисленное значение максимального бюджета сборки дайсов игрока</returns>
        public int GetUsedDiceBudgetByPool(Inventory pool)
        {
            if (pool == null)
            {
                ServiceDebug.LogError("Передан невалидный пул");
                return 0;
            }

            int maxBudget = 0;
            foreach (InventoryCell cell in pool.Data)
            {
                if (cell.GetItemData() is DiceData item)
                {
                    int diceBudgetPrice = item.BudgetPrice;
                    if (diceBudgetPrice > maxBudget) maxBudget = diceBudgetPrice;
                }
            }

            return maxBudget;
        }

        #endregion
    }
}