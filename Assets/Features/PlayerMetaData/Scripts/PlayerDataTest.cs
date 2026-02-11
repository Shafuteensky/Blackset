using System;
using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Inventory.Cells;
using Extensions.Log;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blackset.Player
{
    public class PlayerDataTest : MonoBehaviour
    {
        [Header("Реестры игровых данных"), Space]
        [SerializeField]
        protected DataRegistriesFacade gameDataRegistry;
        
        [Header("Контейнера данных игрока"), Space]
        [SerializeField]
        protected PlayerDataFacade playerDataFacade;
        
        [Header("Предметы на добавление"), Space]
        [SerializeField]
        [Min(0)]
        protected int money = 1;
        
        private void OnEnable()
        {
            // Получение данных
            PlayerMetaData metaData = playerDataFacade.MetaData.Data;
            List<DiceItemCell> dicesInventoryData = playerDataFacade.DicesInventory.Data;
            
            // Вывод данных игрока до обновления
            ServiceDebug.Log($"Текущие данные игрока:" +
                             $"\n{metaData.Money}$" +
                             $"\n{dicesInventoryData.Count} дайсов: {GetDicesInventoryList()}");
            
            // Обновление мета-данных и инвентарей игрока
            // Деньги
            metaData.AddMoney(money);
            // Случайный дайс
            IReadOnlyList<DiceData> diceData = gameDataRegistry.Dices.Data;
            int randomIndex = Random.Range(0, diceData.Count);
            DiceData randomDice = diceData[randomIndex];
            randomIndex = Random.Range(0, diceData.Count);
            DiceType randomDiceType = gameDataRegistry.DiceTypes.Data[randomIndex];
            playerDataFacade.DicesInventory.Add(new DiceItemCell(randomDice.Id, randomDiceType.Id));
            
            // Вывод данных игрока после обновления
            ServiceDebug.Log($"Данные игрока после обновления:" +
                             $"\n{metaData.Money}$" +
                             $"\n{dicesInventoryData.Count} дайсов: {GetDicesInventoryList()}");
        }

        private string GetDicesInventoryList()
        {
            string dicesInInventory = String.Empty;
            foreach (DiceItemCell cell in playerDataFacade.DicesInventory.Data)
            {
                DiceData diceInCell = cell.GetItemData(gameDataRegistry.Dices);
                DiceType diceType = cell.GetTypeData(gameDataRegistry.DiceTypes);
                dicesInInventory += $"{diceInCell.DataName} [{diceType.DataName}], ";
            }

            return dicesInInventory;
        }
    }
}