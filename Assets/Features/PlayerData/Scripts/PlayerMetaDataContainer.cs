using System;
using Extensions.Data.InMemoryData;
using UnityEngine;

namespace Blackset.Player
{
    /// <summary>
    /// Контейнер мета-данных игрока
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlayerMetaDataContainer),
        menuName = "Blackset/Player/" + nameof(PlayerMetaDataContainer))]
    public class PlayerMetaDataContainer : InMemorySingleDataContainer<PlayerMetaData>
    {   
        #region События

        /// <summary>
        /// Изменение количества софт-валюты
        /// </summary>
        /// <typeparam name="int">Актуальная сумма</typeparam>
        /// <typeparam name="int">Добавленное количество</typeparam>
        /// <typeparam name="int">Сумма до добавления</typeparam>
        public event Action<int, int, int> onMoneyChanged;
        
        /// <summary>
        /// Повышение значения очков опыта
        /// </summary>
        /// <typeparam name="int">Значение на текущем уровне</typeparam>
        /// <typeparam name="int">Добавленное количество</typeparam>
        /// <typeparam name="int">Требование на уровень</typeparam>
        /// <typeparam name="float">Прогресс текущего уровня</typeparam>
        public event Action<int, int, int, float> onExpChanged;
        /// <summary>
        /// Повышение уровня игрока
        /// </summary>
        /// <typeparam name="int">Новый уровень</typeparam>
        /// <typeparam name="int">Предыдущий уровень</typeparam>
        public event Action<int, int> onLvlChanged;
        
        #endregion
        
        #region Очки опыта
        
        /// <summary>
        /// Добавить очки опыта
        /// </summary>
        /// <param name="amount"></param>
        public void AddExperience(int amount)
        {
            if (amount <= 0) return;
            int oldExperience = Data.SumExperience;
            Data.AddExperience(amount);
            
            onExpChanged?.Invoke(Data.GetThisLevelExp(), amount, Data.GetThisLevelRequiredExp(), Data.GetProgressToNextLevel());

            int newPlayerLevel = Data.GetPlayerLvl();
            if (Data.PlayerLevelCounter < newPlayerLevel)
            {
                int oldPlayerLevel = Data.PlayerLevelCounter;
                Data.SetLevelCounter(newPlayerLevel);
                
                onLvlChanged?.Invoke(newPlayerLevel, oldPlayerLevel);
            }
            
            MarkDirty();
        }
        
        #endregion
        
        #region Софт-валюта

        /// <summary>
        /// Добавить деньги
        /// </summary>
        /// <param name="amount"></param>
        public void AddMoney(int amount)
        {
            if (amount <= 0) return;
            int oldMoney = Data.Money;
            Data.AddMoney(amount);
            
            onMoneyChanged?.Invoke(Data.Money, amount, oldMoney);
            MarkDirty();
        }
        
        /// <summary>
        /// Убавить деньги
        /// </summary>
        /// <param name="amount"></param>
        public void RemoveMoney(int amount)
        {
            if (amount <= 0) return;
            int oldMoney = Data.Money;
            Data.RemoveMoney(amount);
            
            onMoneyChanged?.Invoke(Data.Money, -amount, oldMoney);
            MarkDirty();
        }
        
        #endregion
    }
}