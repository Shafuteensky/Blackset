using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.DuelScore;
using Blackset.Inventories;
using Blackset.Inventories.Items;
using Blackset.Opponents;
using Blackset.Player;
using Blackset.Rewards;
using Blackset.Shop;
using Blackset.Storms;
using Extensions.Log;
using Extensions.Singleton;
using Features.Progression;
using UnityEngine;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Хелпер для получения данных из editor-настроенных реестров по идентификаторам
    /// </summary>
    public sealed class GameData : MonoBehaviourSingleton<GameData>
    {
        [field: Header("Конфигурации баланса"), Space]

        /// <summary>
        /// Конфигурация прогресса (опыта) игрока
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Конфигурация прогресса игрока (баланса)")]
        public ProgressionConfig ProgressionConfig { get; private set; }
        
        /// <summary>
        /// Конфигурация баланса наград за дуэли
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Конфигурация баланса наград за дуэли")]
        public RewardConfig RewardConfig { get; private set; }
        
        /// <summary>
        /// Конфигурация баланса заработка очков дуэли
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Конфигурация баланса заработка очков дуэли")]
        public DuelScoreConfig DuelScoreConfig { get; private set; }
        
        /// <summary>
        /// Конфигурация баланса магазина
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Конфигурация баланса магазина")]
        public ShopConfig ShopConfig { get; private set; }
        
        // ==============================================
        [field: Header("Постоянные данные игрока"), Space]

        /// <summary>
        /// Фасад данных игрока
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Фасад данных игрока")]
        public PlayerDataFacade PlayerDataFacade { get; private set; }
        
        /// <summary>
        /// Инвентарь для показа новых предметов
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Инвентарь показа новых предметов")]
        public Inventory NewItemsPresenterInventory { get; private set; }
        
        // ==============================================
        [field: Header("Реестры предметов"), Space]

        /// <summary>
        /// Реестр дайсов
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Реестр дайсов")]
        public InventoryItemsRegistry Dices { get; private set; }

        /// <summary>
        /// Реестр расходников
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Реестр расходников")]
        public InventoryItemsRegistry Consumables { get; private set; }

        // -----------------------------------------------
        [field: Header("Реестры типов предметов"), Space]

        /// <summary>
        /// Реестр типов дайсов
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Реестр типов дайсов")]
        public InventoryItemTypesRegistry DiceTypes { get; private set; }

        /// <summary>
        /// Реестр типов расходников
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Реестр типов расходников")]
        public InventoryItemTypesRegistry ConsumableTypes { get; private set; }

        // -----------------------------------------------
        [field: Header("Реестр данных дуэлей"), Space]

        /// <summary>
        /// Реестр соперников
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Реестр соперников")]
        public OpponentsRegistry Opponents { get; private set; }

        /// <summary>
        /// Реестр штормов
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Реестр штормов")]
        public StormsRegistry Storms { get; private set; }

        #region Методы для получения конкретных данных

        public OpponentData GetOpponent(string id) => GetById(Opponents, id);

        public DiceData GetDice(string id) => GetInventoryItem<DiceData>(Dices, id);

        public ConsumableData GetConsumable(string id) => GetInventoryItem<ConsumableData>(Consumables, id);

        public DiceType GetDiceType(string id) => GetInventoryItemType<DiceType>(DiceTypes, id);

        public ConsumableType GetConsumableType(string id) => GetInventoryItemType<ConsumableType>(ConsumableTypes, id);

        public Storm GetStorm(string id) => GetById(Storms, id);

        #endregion

        #region Основные геттеры

        /// <summary>
        /// Получить данные по id из указанного реестра
        /// </summary>
        public TData GetById<TData>(BaseDataRegistry<TData> registry, string id)
            where TData : BaseData
        {
            if (!TryGetById(registry, id, out TData result)) ServiceDebug.LogError($"Данные с id «{id}» не найдены в реестре {registry.name}");
            return result;
        }

        /// <summary>
        /// Попытаться получить данные по id из указанного реестра
        /// </summary>
        public bool TryGetById<TData>(BaseDataRegistry<TData> registry, string id, out TData result)
            where TData : BaseData
        {
            result = null;

            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            if (registry == null)
            {
                ServiceDebug.LogError($"Реестр не задан в {nameof(GameData)}");
                return false;
            }

            result = registry.GetById(id);
            return result != null;
        }

        #endregion

        #region Хелперы реестров игровых данных (геттеры)

        private TExpected GetInventoryItem<TExpected>(InventoryItemsRegistry registry, string id)
            where TExpected : InventoryItem
        {
            TryGetInventoryItem(registry, id, out TExpected result);
            return result;
        }

        private bool TryGetInventoryItem<TExpected>(InventoryItemsRegistry registry, string id, out TExpected result)
            where TExpected : InventoryItem
        {
            result = null;

            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            if (registry == null)
            {
                ServiceDebug.LogError($"Реестр предметов не задан в {nameof(GameData)}");
                return false;
            }

            InventoryItem item = registry.GetById(id);
            if (item == null)
            {
                return false;
            }

            result = item as TExpected;
            if (result == null)
            {
                ServiceDebug.LogError($"Несовпадение типа предмета по id='{id}'. Ожидался {typeof(TExpected).Name}, получен {item.GetType().Name}");
                return false;
            }

            return true;
        }

        private TExpected GetInventoryItemType<TExpected>(InventoryItemTypesRegistry registry, string id)
            where TExpected : InventoryItemType
        {
            TryGetInventoryItemType(registry, id, out TExpected result);
            return result;
        }

        private bool TryGetInventoryItemType<TExpected>(InventoryItemTypesRegistry registry, string id, out TExpected result)
            where TExpected : InventoryItemType
        {
            result = null;

            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            if (registry == null)
            {
                ServiceDebug.LogError($"Реестр типов предметов не задан в {nameof(GameData)}");
                return false;
            }

            InventoryItemType type = registry.GetById(id);
            if (type == null)
            {
                return false;
            }

            result = type as TExpected;
            if (result == null)
            {
                ServiceDebug.LogError($"Несовпадение типа предмета (type) по id='{id}'. Ожидался {typeof(TExpected).Name}, получен {type.GetType().Name}");
                return false;
            }

            return true;
        }

        #endregion
    }
}