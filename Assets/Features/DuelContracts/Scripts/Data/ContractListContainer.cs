using System;
using Blackset.Data.Base;
using Blackset.Opponents;
using Extensions.Log;
using UnityEngine;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// Хранилище доступных соперников
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ContractListContainer),
        menuName = "Blackset/Opponents/" + nameof(ContractListContainer))]
    public class ContractListContainer : RegistrableDataContainer<DuelContract, OpponentData, OpponentsRegistry>
    {
        /// <summary>
        /// Получить данные о сопернике контракта
        /// </summary>
        /// <param name="contractId">Идентификатор контракта</param>
        /// <returns>Данные о сопернике контракта</returns>
        public OpponentData GetOpponentData(string contractId)
        {
            if (String.IsNullOrEmpty(contractId))
            {
                ServiceDebug.LogError("Невалидный идентификатор, данные не переданы");
                return null;
            }
            
            string opponentId = GetById(contractId).OpponentId;
            OpponentData opponentData = GetItemDataById(opponentId);
            return opponentData;
        }
    }
}