using System;
using System.Collections.Generic;
using Extensions.Log;
using Features.Duel.Modules;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Реестр модулей обработки данных дуэли
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(DuelModuleRegistry),
        menuName = "Blackset/Duel/" + nameof(DuelModuleRegistry))]
    public sealed class DuelModuleRegistry : ScriptableObject
    {
        [Header("Модули"), Space]
        [SerializeField]
        private List<BaseDuelModule> modules = new List<BaseDuelModule>();

        private Dictionary<Type, BaseDuelModule> cacheByInterface;
        private bool isCacheBuilt;
        
        private void OnEnable()
        {
            InvalidateCache();
        }

        private void OnValidate()
        {
            ValidateRegistry();
            InvalidateCache();
        }

        #region Получение модулей
        
        /// <summary>
        /// Получить модуль, реализующий указанный интерфейс
        /// </summary>
        /// <typeparam name="TInterface">Интерфейс модуля (наследник <see cref="IDuelModuleInterface"/>)</typeparam>
        /// <returns>Реализация запрашиваемого модуля</returns>
        /// <exception cref="InvalidOperationException">Если модуль не найден или реестр некорректен</exception>
        public TInterface Get<TInterface>() where TInterface : class, IDuelModuleInterface
        {
            BuildCacheIfNeeded();

            Type interfaceType = typeof(TInterface);
            if (cacheByInterface.TryGetValue(interfaceType, out BaseDuelModule module) == false || module == null)
            {
                throw new InvalidOperationException(
                    $"DuelModuleRegistry: модуль для интерфейса '{interfaceType.FullName}' не найден.");
            }

            TInterface casted = module as TInterface;
            if (casted == null)
            {
                throw new InvalidOperationException(
                    $"DuelModuleRegistry: модуль '{module.name}' не приводится к '{interfaceType.FullName}'.");
            }

            return casted;
        }

        /// <summary>
        /// Попытка получить модуль по определенному интерфейсу
        /// </summary>
        /// <typeparam name="TInterface">Интерфейс модуля (наследник <see cref="IDuelModuleInterface"/>)</typeparam>
        /// <param name="result">
        /// Реализация запрашиваемого модуля, если метод вернул true; null, если модуль не найден.
        /// </param>
        /// <returns>
        /// true, если модуль найден; иначе false.
        /// </returns>
        public bool TryGet<TInterface>(out TInterface result) where TInterface : class, IDuelModuleInterface
        {
            result = null;

            BuildCacheIfNeeded();

            Type interfaceType = typeof(TInterface);
            if (cacheByInterface.TryGetValue(interfaceType, out BaseDuelModule module) == false || module == null)
            {
                return false;
            }

            result = module as TInterface;
            return result != null;
        }
        
        #endregion

        #region Internal
        
        private void InvalidateCache()
        {
            isCacheBuilt = false;
            
            cacheByInterface?.Clear();
        }

        private void BuildCacheIfNeeded()
        {
            if (isCacheBuilt) return;

            if (cacheByInterface == null) cacheByInterface = new Dictionary<Type, BaseDuelModule>();
            else cacheByInterface.Clear();

            foreach (BaseDuelModule module in modules)
            {
                if (module == null) continue;

                Type contractInterface = GetModuleContractInterface(module);
                if (contractInterface == null) continue;

                if (cacheByInterface.TryGetValue(contractInterface, out BaseDuelModule existing))
                {
                    throw new InvalidOperationException(
                        $"DuelModuleRegistry: найден дубликат интерфейса '{contractInterface.FullName}'. " +
                        $"Модули: '{existing.name}' и '{module.name}'.");
                }

                cacheByInterface.Add(contractInterface, module);
            }

            isCacheBuilt = true;
        }

        private void ValidateRegistry()
        {
            Dictionary<Type, BaseDuelModule> seen = new();

            foreach (BaseDuelModule module in modules)
            {
                if (module == null) continue;

                var contractInterface = GetModuleContractInterface(module);
                if (contractInterface == null)
                {
                    ServiceDebug.LogError(
                        $"Модуль '{module.name}' не реализует ни одного " +
                        $"интерфейса-наследника '{nameof(IDuelModuleInterface)}' (или реализует более одного)");
                    continue;
                }

                if (seen.TryGetValue(contractInterface, out BaseDuelModule existing))
                {
                    ServiceDebug.LogError(
                        $"Дубликат реализации интерфейса '{contractInterface.FullName}' " +
                        $"\nМодули: '{existing.name}' и '{module.name}'");
                    continue;
                }

                seen.Add(contractInterface, module);
            }
        }

        private Type GetModuleContractInterface(BaseDuelModule module)
        {
            Type moduleType = module.GetType();

            Type[] interfaces = moduleType.GetInterfaces();
            List<Type> contractInterfaces = new List<Type>();

            foreach (Type moduleInterface in interfaces)
            {
                if (moduleInterface == typeof(IDuelModuleInterface)) continue;

                if (typeof(IDuelModuleInterface).IsAssignableFrom(moduleInterface)) 
                    contractInterfaces.Add(moduleInterface);
            }

            if (contractInterfaces.Count != 1) return null;
            return contractInterfaces[0];
        }
        
        #endregion
    }
}