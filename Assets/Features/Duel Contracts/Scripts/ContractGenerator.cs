using System.Collections.Generic;
using Blackset.Data.Registries;
using Blackset.Opponents;
using Blackset.OpponentsRestrictions;
using UnityEngine;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// Генератор соперников
    /// </summary>
    public class ContractGenerator
    {
        private const float DEFAULT_DIFFICULTY = 0.5f;
        
        private readonly GameData gameData = GameData.Instance;
        //protected readonly PlayerDataFacade playerData; // TODO данные игрока для определения доступности предметов от стадии прогресса
        
        /// <summary>
        /// Конструктор генератора предметов
        /// </summary>
        /// <param name="gameData">Фасад всех игровых данных</param>
        /// <param name="gameData">Фасад всех данных игрока</param>
        public ContractGenerator()
        {
            //this.playerData = playerData;
        }
        
        /// <summary>
        /// Получить случайный контракт со случайным соперником из реестра всех существующих
        /// </summary>
        public DuelContract GetRandomOpponent()
        {
            OpponentData randomOpponent = gameData.Opponents.Data[Random.Range(0, gameData.Opponents.Data.Count)];
            DuelContract randomContract = new DuelContract(randomOpponent);
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

            List<OpponentData> availableOpponents = gameData.Opponents.GetUnrestricted(OpponentAvailability.Duel);
            int opponentsCount = availableOpponents.Count;

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

                OpponentData opponent = availableOpponents[indices[i]];
                result.Add(new DuelContract(opponent));
            }

            // 3) Если нужно больше, чем есть — добираем с повторами
            // if (number > opponentsCount)
            // {
            //     for (int i = uniqueToTake; i < number; i++)
            //     {
            //         OpponentData opponent = availableOpponents[Random.Range(0, opponentsCount)];
            //         result.Add(new DuelContract(opponent));
            //     }
            // }

            return result;
        }
    }
}