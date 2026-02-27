using System;
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
        public string OpponentId => opponentId;
        /// <summary>
        /// Оппонент
        /// </summary>
        public OpponentData Opponent => opponentsRegistry.GetById(opponentId);

        private readonly string opponentId;
        private readonly OpponentsRegistry opponentsRegistry;
        
        /// <summary>
        /// Конструктор записи о сопернике
        /// </summary>
        /// <param name="opponentId">Идентификатор данных оппонента этого контракта</param>
        public DuelContract(string opponentId, OpponentsRegistry opponentsRegistry) 
        {
            if (String.IsNullOrEmpty(opponentId) || opponentsRegistry == null)
            {
                ServiceDebug.LogError("Невалидный id оппонента или ссылка на реестр при создании контракта");
                opponentId = String.Empty;
            }
            
            this.opponentId = opponentId;
            this.opponentsRegistry = opponentsRegistry;
        }
    }
}