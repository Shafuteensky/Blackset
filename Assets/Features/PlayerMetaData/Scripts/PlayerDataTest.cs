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
        protected DiceDataRegistry diceDataRegistry;
        [SerializeField]
        protected DiceTypeRegistry diceTypeDataRegistry;
        
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
            int randomIndex = Random.Range(0, diceDataRegistry.Data.Count);
            DiceData randomDice = diceDataRegistry.Data[randomIndex];
            randomIndex = Random.Range(0, diceTypeDataRegistry.Data.Count);
            DiceType randomDiceType = diceTypeDataRegistry.Data[randomIndex];
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
                DiceData diceInCell = cell.GetItemData(diceDataRegistry);
                DiceType diceType = cell.GetTypeData(diceTypeDataRegistry);
                dicesInInventory += $"{diceInCell.DataName} [{diceType.DataName}], ";
            }

            return dicesInInventory;
        }
    }
}