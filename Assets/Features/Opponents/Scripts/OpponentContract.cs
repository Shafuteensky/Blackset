using System;
using Extensions.Data.InMemoryData;
using Extensions.Log;

namespace Blackset.Opponents
{
    /// <summary>
    /// Запись о сопернике в пуле доступных контрактов
    /// </summary>
    public class OpponentContract : InMemoryDataEntry
    {
        /// <summary>
        /// Идентификатор данных оппонента
        /// </summary>
        public string OpponentId { get => opponentId; private set => opponentId = value; }

        protected string opponentId;
        
        /// <summary>
        /// Конструктор записи о сопернике
        /// </summary>
        /// <param name="opponentId">Идентификатор данных оппонента этого контракта</param>
        public OpponentContract(string opponentId)
        {
            if (String.IsNullOrEmpty(opponentId))
            {
                ServiceDebug.LogError("Невалидный id оппонента при создании записи контракта");
                opponentId = String.Empty;
            }
            
            this.opponentId = opponentId;
        }
    }
}