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
            IReadOnlyList<InventoryItem> diceData = gameDataRegistry.Dices.Data;
            List<InventoryCell> dicesInventoryData = playerDataFacade.DicesInventory.Data;
            
            int randomIndex = Random.Range(0, diceData.Count);
            InventoryItem randomItem = diceData[randomIndex];

            if (randomItem is not DiceData randomDiceItem) return;
            randomIndex = Random.Range(0, randomDiceItem.AvailableTypes.Count);
            InventoryItemType randomDiceType = randomDiceItem.AvailableTypes[randomIndex];
            
            playerDataFacade.DicesInventory.AddItem(randomDiceItem.Id, randomDiceType.Id, 1);
            
            ServiceDebug.Log($"Добавлен новый дайс: {randomDiceItem.DataName}, {randomDiceType.DataName}. " +
                             $"\nТеперь в инвентаре {dicesInventoryData.Count} дайсов: {GetDicesInventoryList(playerDataFacade.DicesInventory, "    ")}");
        }

        [ContextMenu("Add Random Consumable")]
        public void AddRandomConsumable()
        {
            IReadOnlyList<InventoryItem> consumablesData = gameDataRegistry.Consumables.Data;
            List<InventoryCell> consumabledInventoryData = playerDataFacade.ConsumablesInventory.Data;
            
            int randomIndex = Random.Range(0, consumablesData.Count);
            InventoryItem randomItem = consumablesData[randomIndex];

            if (randomItem is not ConsumableData randomConsumable) return;
            randomIndex = Random.Range(0, gameDataRegistry.ConsumableTypes.Data.Count);
            InventoryItemType randomConsumableType = gameDataRegistry.ConsumableTypes.Data[randomIndex];
            
            playerDataFacade.ConsumablesInventory.AddItem(randomConsumable.Id, randomConsumableType.Id, 1);
            
            ServiceDebug.Log($"Добавлен новый расходник: {randomConsumable.DataName}, {randomConsumableType.DataName}. " +
                             $"\nТеперь в инвентаре {consumabledInventoryData.Count} расходников: " +
                             $"{GetConsumablesInventoryList(playerDataFacade.ConsumablesInventory, "    ")}");
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
            List<InventoryCell> dicesInventoryData = playerDataFacade.DicesInventory.Data;
            List<InventoryCell> consumablesInventoryData = playerDataFacade.ConsumablesInventory.Data;

            ServiceDebug.Log($"Данные игрока:" +
                             $"\nВалюта: {metaData.Money}$" +
                             $"\nОпыт: {metaData.GetThisLevelExp()}/{metaData.GetThisLevelRequiredExp()}" +
                             $"\nУровень: {metaData.GetPlayerLvl()}" +
                             $"\nИнвантарь дайсов ({dicesInventoryData.Count} в сумме):" +
                             $"{GetDicesInventoryList(playerDataFacade.DicesInventory, "    ")}" +
                             $"\nПулы дайсов:" + GetDicesPoolList("        ") +
                             $"\nИнвентарь расходников ({consumablesInventoryData.Count} в сумме): " +
                             $"{GetConsumablesInventoryList(playerDataFacade.ConsumablesInventory, "    ")}" +
                             $"\nПул расходников:" + GetConsumablesInventoryList(playerDataFacade.ConsumablesPool, "    ")
                             );
        }

        private string GetDicesInventoryList(Inventory inventory, string prefix = "")
        {
            string dicesInInventory = String.Empty;
            foreach (InventoryCell cell in inventory.Data)
            {
                InventoryItem diceInCell = cell.GetItemData(gameDataRegistry.Dices);
                if (diceInCell == null) continue;
                InventoryItemType diceType = cell.GetTypeData(gameDataRegistry.DiceTypes);
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
                InventoryItem consumableInCell = cell.GetItemData(gameDataRegistry.Consumables);
                if (consumableInCell == null) continue;
                InventoryItemType consumableType = cell.GetTypeData(gameDataRegistry.ConsumableTypes);
                consumablesInInventory += $"\n{prefix}- {consumableInCell.DataName}, {consumableType.DataName} ({cell.ItemAmount} шт)";
            }
            return consumablesInInventory;
        }
    }
}