using System;
using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;
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

        [ContextMenu("Add Random Dice")]
        public void AddRandomDice()
        {
            IReadOnlyList<DiceData> diceData = gameDataRegistry.Dices.Data;
            List<DiceItemCell> dicesInventoryData = playerDataFacade.DicesInventory.Data;
            
            int randomIndex = Random.Range(0, diceData.Count);
            DiceData randomDice = diceData[randomIndex];
            randomIndex = Random.Range(0, randomDice.AvailableTypes.Count);
            DiceType randomDiceType = randomDice.AvailableTypes[randomIndex];
            
            playerDataFacade.DicesInventory.AddItem(new DiceItemCell(randomDice.Id, randomDiceType.Id));
            
            ServiceDebug.Log($"Добавлен новый дайс: {randomDice.DataName}, {randomDiceType.DataName}. " +
                             $"\nТеперь в инвентаре {dicesInventoryData.Count} дайсов: {GetDicesInventoryList(playerDataFacade.DicesInventory, "    ")}");
        }

        [ContextMenu("Add 10 Money")]
        public void AddSomeMoney() => AddMoney();
        public void AddMoney(int moneyToAdd = 10)
        {
            PlayerMetaData metaData = playerDataFacade.MetaData.Data;
            
            playerDataFacade.MetaData.AddMoney(moneyToAdd);
            ServiceDebug.Log($"Добавлена валюта ({moneyToAdd}, в сумме {playerDataFacade.MetaData.Data.Money})");
        }

        [ContextMenu("Print All Player Data")]
        public void PrintPlayerData()
        {
            PlayerMetaData metaData = playerDataFacade.MetaData.Data;
            List<DiceItemCell> dicesInventoryData = playerDataFacade.DicesInventory.Data;

            ServiceDebug.Log($"Данные игрока:" +
                             $"\nВалюта: {metaData.Money}$" +
                             $"\nИнвантарь:" +
                             $"\n   - Дайсы ({dicesInventoryData.Count} в сумме): {GetDicesInventoryList(playerDataFacade.DicesInventory, "        ")}" +
                             $"\nПулы дайсов:" + GetDicesPoolList("        ")
                             );
        }

        private string GetDicesInventoryList(DicesInventory inventory, string prefix = "")
        {
            string dicesInInventory = String.Empty;
            foreach (DiceItemCell cell in inventory.Data)
            {
                DiceData diceInCell = cell.GetItemData(gameDataRegistry.Dices);
                if (diceInCell == null) continue;
                DiceType diceType = cell.GetTypeData(gameDataRegistry.DiceTypes);
                dicesInInventory += $"\n{prefix}- {diceInCell.DataName}, {diceType.DataName} ({cell.ItemAmount} шт)";
            }

            return dicesInInventory;
        }

        private string GetDicesPoolList(string prefix = "")
        {
            string dicesInPools = String.Empty;
            foreach (DicesInventory poolRow in playerDataFacade.DicesPoolRows)
            {
                dicesInPools += $"\n   - {poolRow.AllowedItemType.DataName} ({poolRow.Data.Count} в сумме): ";
                dicesInPools += GetDicesInventoryList(poolRow, prefix);
            }
            return dicesInPools;
        }
    }
}