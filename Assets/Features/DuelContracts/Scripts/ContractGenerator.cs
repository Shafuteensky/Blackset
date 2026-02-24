using System.Collections.Generic;
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
        /// Получить случайного соперника из реестра всех существующих
        /// </summary>
        public DuelContract GetRandomOpponent()
        {
            OpponentData randomOpponent = opponentsRegistry.Data[Random.Range(0, opponentsRegistry.Data.Count)];
            DuelContract randomContract = new DuelContract(randomOpponent.Id);
            return randomContract;
        }
        
        /// <summary>
        /// Получить список случайных контрактов заданного размера
        /// </summary>
        /// <remarks>
        /// Соперники в списке не повторяются, кроме случая когда number больше, чем количество соперников в реестре
        /// </remarks>
        public List<DuelContract> GetRandomContractsList(int number)
        {
            List<DuelContract> result = new List<DuelContract>();

            if (number <= 0) return result;

            if (opponentsRegistry == null)
            {
                ServiceDebug.LogError("Ссылка на реестр данных не получена");
                return result;
            }

            if (opponentsRegistry.Data == null || opponentsRegistry.Data.Count == 0)
            {
                ServiceDebug.LogError("Реестр соперников пуст");
                return result;
            }

            int opponentsCount = opponentsRegistry.Data.Count;

            // 1) Сколько можем выдать без повторов
            int uniqueToTake = Mathf.Min(number, opponentsCount);

            // 2) Берём уникальных через перемешивание индексов (Fisher–Yates частично)
            List<int> indices = new List<int>(opponentsCount);
            for (int i = 0; i < opponentsCount; i++)
            {
                indices.Add(i);
            }

            for (int i = 0; i < uniqueToTake; i++)
            {
                int swapIndex = Random.Range(i, opponentsCount);

                (indices[i], indices[swapIndex]) = (indices[swapIndex], indices[i]);

                OpponentData opponent = opponentsRegistry.Data[indices[i]];
                result.Add(new DuelContract(opponent.Id));
            }

            // 3) Если нужно больше, чем есть — добираем с повторами
            if (number > opponentsCount)
            {
                for (int i = uniqueToTake; i < number; i++)
                {
                    OpponentData opponent = opponentsRegistry.Data[Random.Range(0, opponentsCount)];
                    result.Add(new DuelContract(opponent.Id));
                }
            }

            return result;
        }
    }
}