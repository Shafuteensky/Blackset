using Extensions.Log;
using UnityEngine;

namespace Blackset.Opponents
{
    /// <summary>
    /// Генератор соперников
    /// </summary>
    public class OpponentGenerator
    {   
        protected readonly OpponentsRegistry opponentsRegistry;
        //protected readonly PlayerDataFacade playerData; // TODO данные игрока для определения доступности предметов от стадии прогресса
        
        /// <summary>
        /// Конструктор генератора предметов
        /// </summary>
        /// <param name="gameData">Фасад всех игровых данных</param>
        /// <param name="gameData">Фасад всех данных игрока</param>
        public OpponentGenerator(OpponentsRegistry opponentsRegistry)
        {
            if (opponentsRegistry == null) ServiceDebug.LogError("Ссылка на реестр данных не получена");
            
            this.opponentsRegistry = opponentsRegistry;
            //this.playerData = playerData;
        }
        
        /// <summary>
        /// Получить случайного соперника
        /// </summary>
        public OpponentContract GetRandomOpponent()
        {
            OpponentData randomOpponent = opponentsRegistry.Data[Random.Range(0, opponentsRegistry.Data.Count)];
            OpponentContract randomContract = new OpponentContract(randomOpponent.Id);
            return randomContract;
        }
    }
}