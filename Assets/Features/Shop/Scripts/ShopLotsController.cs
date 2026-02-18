using Blackset.Data.Registries;
using Blackset.Inventory.Inventories;
using Blackset.Player;
using Extensions.Data;
using Extensions.Generics;
using Extensions.Identification;
using Extensions.Log;
using Features.ItemGenerators;
using UnityEngine;

namespace Features.Shop
{
    /// <summary>
    /// Контроллер магазина предметов
    /// </summary>
    public sealed class ShopLotsController : InitializableMonoBehaviour
    {
        [Header("Сохранение данных"), Space] [SerializeField]
        private ID identifier;
        
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
        private DicesInventory shopDicesInventory;
        [SerializeField]
        private ConsumablesInventory shopConsumablesInventory;
        [SerializeField]
        private DicesInventory shopDicesBoxInventory;
        [SerializeField]
        private ConsumablesInventory shopConsumablesBoxInventory;
        
        [Header("Игровые данные"), Space]
        [SerializeField]
        private DataRegistriesFacade gameData;
        [SerializeField]
        private PlayerDataFacade playerData;
        
        private ItemsGenerator itemsGenerator;
        private int cachedDuelsPlayed;

        private void Awake()
        {
            if (gameData == null) return;
            itemsGenerator ??= new ItemsGenerator(gameData);
        }
        
        private void OnEnable()
        {
            if (gameData == null)
            {
                ServiceDebug.LogError("Ссылка на реестр данных отсутствует");
                return;
            }

            
            Initialize(gameData != null && playerData != null && 
                       shopDicesInventory != null && shopConsumablesInventory != null && 
                       shopDicesBoxInventory != null && shopConsumablesBoxInventory != null && 
                       itemsGenerator != null);

            if (IsPlayerProgressChanged()) FillInventories();
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
        
        private void RefillDiceInventory(DicesInventory inventory, int amount)
        {
            inventory.Clear();

            for (int i = 0; i < amount; i++)
            {
                var newDice = itemsGenerator.GetRandomDice();
                inventory.AddItem(newDice.Item.Id, newDice.Type.Id, 1, false);
            }
        }

        private void RefillConsumableInventory(ConsumablesInventory inventory, int amount)
        {
            inventory.Clear();

            for (int i = 0; i < amount; i++)
            {
                var newConsumable = itemsGenerator.GetRandomConsumable();
                inventory.AddItem(newConsumable.Item.Id, newConsumable.Type.Id, 1, false);
            }
        }
        
        #endregion

        private bool IsPlayerProgressChanged()
        {
            if (identifier == null)
            {
                ServiceDebug.LogError("Идентификатор ключа сохранения не задан");
                return false;
            }
            
            int duelsPlayed = playerData.ProgressData.Data.DuelsPlayed;
            cachedDuelsPlayed = JsonSaveLoad.Load(identifier.Id, -1);

            if (duelsPlayed == cachedDuelsPlayed) return false;
            
            cachedDuelsPlayed = duelsPlayed;
            JsonSaveLoad.Save(cachedDuelsPlayed, identifier.Id);
            return true;
        }
    }
}