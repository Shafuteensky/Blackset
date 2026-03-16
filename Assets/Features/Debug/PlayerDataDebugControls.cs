using System;
using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Blackset.Inventories.Items;
using Blackset.Player;
using Extensions.Log;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blackset.GameDebug
{
    public class PlayerDataDebugControls : MonoBehaviour
    {
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
            IReadOnlyList<InventoryItem> allDices = GameData.Instance.Dices.Data;
            List<InventoryCell> dicesInventoryData = playerDataFacade.Inventory.Data;
            
            int randomIndex = Random.Range(0, allDices.Count);
            InventoryItem randomItem = allDices[randomIndex];

            if (randomItem is not DiceData randomDiceItem) return;
            randomIndex = Random.Range(0, randomDiceItem.AvailableTypes.Count);
            InventoryItemType randomDiceType = randomDiceItem.AvailableTypes[randomIndex];

            ItemContext diceItem = new ItemContext(randomDiceItem.Id, randomDiceType.Id, randomDiceItem.ItemClass);
            playerDataFacade.Inventory.AddItem(diceItem, 1);
            
            ServiceDebug.Log($"Добавлен новый дайс: {randomDiceItem.DataName}, {randomDiceType.DataName}. " +
                             $"\nТеперь в инвентаре {dicesInventoryData.Count} дайсов: {GetDicesInventoryList(playerDataFacade.Inventory, "    ")}");
        }

        [ContextMenu("Add Random Consumable")]
        public void AddRandomConsumable()
        {
            IReadOnlyList<InventoryItem> allConsumables = GameData.Instance.Consumables.Data;
            List<InventoryCell> consumabledInventoryData = playerDataFacade.Inventory.Data;
            
            int randomIndex = Random.Range(0, allConsumables.Count);
            InventoryItem randomItem = allConsumables[randomIndex];

            if (randomItem is not ConsumableData randomConsumable) return;
            randomIndex = Random.Range(0, GameData.Instance.ConsumableTypes.Data.Count);
            InventoryItemType randomConsumableType = GameData.Instance.ConsumableTypes.Data[randomIndex];
            
            ItemContext diceConsumable = new ItemContext(randomItem.Id, randomConsumableType.Id, randomItem.ItemClass);
            playerDataFacade.Inventory.AddItem(diceConsumable, 1);
            
            ServiceDebug.Log($"Добавлен новый расходник: {randomConsumable.DataName}, {randomConsumableType.DataName}. " +
                             $"\nТеперь в инвентаре {consumabledInventoryData.Count} расходников: " +
                             $"{GetConsumablesInventoryList(playerDataFacade.Inventory, "    ")}");
        }
        
        [ContextMenu("Add 10 Money")]
        public void AddSomeMoney() => AddMoney();
        public void AddMoney(int moneyToAdd = 10)
        {
            PlayerMetaData metaData = playerDataFacade.MetaData.Data;
            
            playerDataFacade.MetaData.AddMoney(moneyToAdd);
            ServiceDebug.Log($"Добавлена валюта ({moneyToAdd}, в сумме {metaData.Money})");
        }

        [ContextMenu("Add Experience")]
        public void AddExp()
        {
            PlayerMetaData metaData = playerDataFacade.MetaData.Data;
            
            int expToAdd = Random.Range( metaData.GetThisLevelRequiredExp()/12, metaData.GetThisLevelRequiredExp()/8 );
            playerDataFacade.MetaData.AddExperience(expToAdd);
            ServiceDebug.Log($"Добавлен опыт ({expToAdd}, до нового уровня {metaData.GetExpToNextLevel()}, в сумме {metaData.SumExperience})");
        }

        [ContextMenu("Print All Player Data")]
        public void PrintPlayerData()
        {
            PlayerMetaData metaData = playerDataFacade.MetaData.Data;
            List<InventoryCell> dicesInventoryData = playerDataFacade.Inventory.Data;
            List<InventoryCell> consumablesInventoryData = playerDataFacade.Inventory.Data;

            ServiceDebug.Log($"Данные игрока:" +
                             $"\nВалюта: {metaData.Money}$" +
                             $"\nОпыт: {metaData.GetThisLevelExp()}/{metaData.GetThisLevelRequiredExp()}" +
                             $"\nУровень: {metaData.GetPlayerLvl()}" +
                             $"\nИнвантарь ({dicesInventoryData.Count} в сумме):" +
                             $"{GetDicesInventoryList(playerDataFacade.Inventory, "    ")}" +
                             $"\nПулы дайсов:" + GetDicesPoolList("        ") +
                             $"\nПул расходников:" + GetConsumablesInventoryList(playerDataFacade.ConsumablesPool, "    ")
                             );
        }

        private string GetDicesInventoryList(Inventory inventory, string prefix = "")
        {
            string dicesInInventory = String.Empty;
            foreach (InventoryCell cell in inventory.Data)
            {
                InventoryItem diceInCell = cell.GetItemData();
                if (diceInCell == null) continue;
                InventoryItemType diceType = cell.GetTypeData();
                dicesInInventory += $"\n{prefix}- {diceInCell.DataName}, {diceType.DataName} ({cell.ItemAmount} шт)";
            }
            return dicesInInventory;
        }

        private string GetDicesPoolList(string prefix = "")
        {
            string dicesInPools = String.Empty;
            foreach (Inventory poolRow in playerDataFacade.DicesPoolRows)
            {
                dicesInPools += $"\n   - {poolRow.AllowedItemType.DataName} ({poolRow.Data.Count} в сумме): ";
                dicesInPools += GetDicesInventoryList(poolRow, prefix);
            }
            return dicesInPools;
        }

        private string GetConsumablesInventoryList(Inventory inventory, string prefix = "")
        {
            string consumablesInInventory = String.Empty;
            foreach (InventoryCell cell in inventory.Data)
            {
                InventoryItem consumableInCell = cell.GetItemData();
                if (consumableInCell == null) continue;
                InventoryItemType consumableType = cell.GetTypeData();
                consumablesInInventory += $"\n{prefix}- {consumableInCell.DataName}, {consumableType.DataName} ({cell.ItemAmount} шт)";
            }
            return consumablesInInventory;
        }
    }
}