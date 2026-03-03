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
        
        private readonly string opponentId;
        
        /// <summary>
        /// Конструктор записи о сопернике
        /// </summary>
        /// <param name="opponentId">Идентификатор данных оппонента этого контракта</param>
        public DuelContract(string opponentId) 
        {
            if (String.IsNullOrEmpty(opponentId))
            {
                ServiceDebug.LogError("Невалидный id оппонента или ссылка на реестр при создании контракта");
                opponentId = String.Empty;
            }
            
            this.opponentId = opponentId;
        }
    }
}