using System;
using System.Collections.Generic;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.Inventories.Items;
using Blackset.Opponents;
using Blackset.Storms;
using Extensions.Log;
using Extensions.Singleton;
using UnityEngine;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Хелпер для получения данных из editor-настроенных реестров по идентификаторам
    /// </summary>
    public sealed class GameData : MonoBehaviourSingleton<GameData>
    {
        [Header("Реестры данных"), Space]
        [SerializeField]
        private List<BaseDataRegistry> registries = new List<BaseDataRegistry>();

        private Dictionary<Type, BaseDataRegistry> registryByType;

        private void OnEnable() => BuildIndex();

        private void OnValidate() => registryByType = null;

        #region Фасады для получения конкретных данных

        public OpponentData GetOpponent(string Id) => GetById<OpponentsRegistry, OpponentData>(Id);
        
        public DiceData GetDice(string Id) => GetById<DicesDataRegistry, DiceData>(Id);
        public ConsumableData GetConsumable(string Id) => GetById<ConsumablesDataRegistry, ConsumableData>(Id);
        
        public DiceType GetDiceType(string Id) => GetById<DiceTypesDataRegistry, DiceType>(Id);
        public ConsumableType GetConsumableType(string Id) => GetById<ConsumableTypesDataRegistry, ConsumableType>(Id);

        public Storm GetStorm(string Id) => GetById<StormsRegistry, Storm>(Id);
        
        #endregion
        
        
        #region Основные геттеры
        
        /// <summary>
        /// Получить реестр по типу
        /// </summary>
        /// <typeparam name="TRegistry">Тип реестра</typeparam>
        /// <returns>Реестр указанного типа или null</returns>
        public TRegistry GetRegistry<TRegistry>() where TRegistry : BaseDataRegistry
        {
            EnsureIndex();

            Type type = typeof(TRegistry);

            if (registryByType == null)
            {
                ServiceDebug.LogError("Ошибка получения индекса реестров");
                return null;
            }

            if (registryByType.TryGetValue(type, out BaseDataRegistry registry))
            {
                return registry as TRegistry;
            }

            ServiceDebug.LogError($"Реестр типа {type.Name} не найден. Необходимо добавить его в {nameof(GameData)}");
            return null;
        }

        /// <summary>
        /// Попытаться получить данные по id из реестра указанного типа
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="result">Полученные данные</param>
        /// <typeparam name="TRegistry">Тип реестра</typeparam>
        /// <typeparam name="TData">Тип данных</typeparam>
        /// <returns>true, если данные найдены</returns>
        public bool TryGetById<TRegistry, TData>(string id, out TData result)
            where TRegistry : BaseDataRegistry<TData>
            where TData : BaseData
        {
            result = null;

            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            TRegistry registry = GetRegistry<TRegistry>();
            if (registry == null)
            {
                return false;
            }

            result = registry.GetById(id);
            return result != null;
        }

        /// <summary>
        /// Получить данные по id из реестра указанного типа
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <typeparam name="TRegistry">Тип реестра</typeparam>
        /// <typeparam name="TData">Тип данных</typeparam>
        /// <returns>Данные или null</returns>
        public TData GetById<TRegistry, TData>(string id)
            where TRegistry : BaseDataRegistry<TData>
            where TData : BaseData
        {
            TryGetById<TRegistry, TData>(id, out TData result);
            return result;
        }
        
        #endregion

        #region Internal
        
        private void EnsureIndex()
        {
            if (registryByType != null)
            {
                return;
            }

            BuildIndex();
        }

        private void BuildIndex()
        {
            if (registries == null || registries.Count == 0)
            {
                registryByType = null;
                return;
            }

            if (registryByType == null)
            {
                registryByType = new Dictionary<Type, BaseDataRegistry>(registries.Count);
            }
            else
            {
                registryByType.Clear();
            }

            foreach (var registry in registries)
            {
                if (registry == null)
                {
                    continue;
                }

                Type registryType = registry.GetType();

                if (registryByType.ContainsKey(registryType))
                {
                    ServiceDebug.LogError($"Дубликат реестра типа {registryType.Name} в DataRegistryHelper. Оставлен последний");
                }

                registryByType[registryType] = registry;
            }
        }
        
        #endregion
    }
}