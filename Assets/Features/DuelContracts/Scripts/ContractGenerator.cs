using Blackset.Opponents;
using Extensions.Log;
using UnityEngine;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// Генератор соперников
    /// </summary>
    public class ContractGenerator
    {   
        protected readonly OpponentsRegistry opponentsRegistry;
        //protected readonly PlayerDataFacade playerData; // TODO данные игрока для определения доступности предметов от стадии прогресса
        
        /// <summary>
        /// Конструктор генератора предметов
        /// </summary>
        /// <param name="gameData">Фасад всех игровых данных</param>
        /// <param name="gameData">Фасад всех данных игрока</param>
        public ContractGenerator(OpponentsRegistry opponentsRegistry)
        {
            if (opponentsRegistry == null) ServiceDebug.LogError("Ссылка на реестр данных не получена");
            
            this.opponentsRegistry = opponentsRegistry;
            //this.playerData = playerData;
        }
        
        /// <summary>
        /// Получить случайного соперника
        /// </summary>
        public DuelContract GetRandomOpponent()
        {
            OpponentData randomOpponent = opponentsRegistry.Data[Random.Range(0, opponentsRegistry.Data.Count)];
            DuelContract randomContract = new DuelContract(randomOpponent.Id);
            return randomContract;
        }
    }
}