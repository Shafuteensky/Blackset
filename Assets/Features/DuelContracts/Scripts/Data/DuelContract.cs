using System;
using Blackset.Data.Registries;
using Blackset.Opponents;
using Extensions.Data.InMemoryData;
using Extensions.Log;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// Запись о сопернике в пуле доступных контрактов
    /// </summary>
    public class DuelContract : InMemoryDataEntry
    {
        /// <summary>
        /// Идентификатор данных оппонента
        /// </summary>
        public string OpponentId { get; private set; }
        /// <summary>
        /// Награда в валюте
        /// </summary>
        public int MoneyReward { get; private set; }

        /// <summary>
        /// Новый контракт
        /// </summary>
        /// <param name="opponent">Данные соперника</param>
        public DuelContract(OpponentData opponent) 
        {
            if (opponent == null)
            {
                ServiceDebug.LogError("Невалидные данные оппонента при создании контракта");
                return;
            }
            
            OpponentId = opponent.Id;
            int moneyReward = GameData.Instance.RewardConfig.EvaluateMoney(true, opponent);
            MoneyReward = moneyReward;
        }
    }
}