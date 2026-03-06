using Blackset.Data.Registries;
using Blackset.Player;
using Extensions.Log;
using Blacklset.ItemGenerators;
using UnityEngine;
using Blackset.Inventories;

namespace Blackset.Shop
{
    /// <summary>
    /// Контроллер магазина предметов
    /// </summary>
    public sealed class ShopLotsController : PlayerProgressUpdater
    {
        [Header("Ограничения лотов"), Space]
        [SerializeField]
        [Range(1, 6)]
        private int dicesToSell = 1;
        [SerializeField]
        [Range(1, 6)]
        private int consumablesToSell = 1;
        [SerializeField]
        [Range(1, 6)]
        private int dicesPacksToSell = 2;
        [SerializeField]
        [Range(1, 6)]
        private int consumablesPacksToSell = 2;
        
        [Header("Инвентари магазина"), Space]
        [SerializeField]
        private Inventory shopDicesInventory;
        [SerializeField]
        private Inventory shopConsumablesInventory;
        [SerializeField]
        private Inventory shopDicesBoxInventory;
        [SerializeField]
        private Inventory shopConsumablesBoxInventory;
        
        private ItemsGenerator itemsGenerator;
        private readonly GameData gameData = GameData.Instance;
        
        private void OnEnable()
        {
            if (gameData == null)
            {
                ServiceDebug.LogError("Ссылка на реестр данных отсутствует");
                return;
            }

            Initialize(gameData != null && 
                       shopDicesInventory != null && shopConsumablesInventory != null && 
                       shopDicesBoxInventory != null && shopConsumablesBoxInventory != null);
            if (!IsInitialized) return;
            
            itemsGenerator ??= new ItemsGenerator();
            if (IsUpdateNeeded()) FillInventories();
        }

        #region Refilling
        
        private void FillInventories()
        {
            if (!IsInitialized) return;

            RefillDiceInventory(shopDicesInventory, dicesToSell);
            RefillConsumableInventory(shopConsumablesInventory, consumablesToSell);
            
            RefillDiceInventory(shopDicesBoxInventory, dicesPacksToSell);
            RefillConsumableInventory(shopConsumablesBoxInventory, consumablesPacksToSell);
        }
        
        private void RefillDiceInventory(Inventory inventory, int amount)
        {
            inventory.Clear();

            for (int i = 0; i < amount; i++)
            {
                var newDice = itemsGenerator.GetRandomDice();
                inventory.AddItem(newDice.Dice.Id, newDice.Type.Id, 1, false);
            }
        }

        private void RefillConsumableInventory(Inventory inventory, int amount)
        {
            inventory.Clear();

            for (int i = 0; i < amount; i++)
            {
                var newConsumable = itemsGenerator.GetRandomConsumable();
                inventory.AddItem(newConsumable.Consumable.Id, newConsumable.Type.Id, 1, false);
            }
        }
        
        #endregion
    }
}