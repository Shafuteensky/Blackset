using System;
using Blackset.Data.Registries;
using Extensions.Data.InMemoryData;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Data.Base
{
    /// <summary>
    /// Контейнер данных по id с привязкой реестра для получения данных по id
    /// </summary>
    /// <typeparam name="TEntry">Тип записи контейнера</typeparam>
    /// <typeparam name="TItem">Тип хранимых данных в записи</typeparam>
    /// <typeparam name="TRegistry">Тип реестра, хоранящего данные типа TItem</typeparam>
    public abstract class RegistrableDataContainer<TEntry, TItem, TRegistry> : InMemoryDataContainer<TEntry> 
        where TEntry : InMemoryDataEntry
        where TItem : BaseData
        where TRegistry : BaseDataRegistry<TItem>
    {
        /// <summary>
        /// Реестр данных предметов для разрешения itemId -> данные
        /// </summary>
        public TRegistry DataRegistry => dataRegistry;
        
        [Header("Реестры игровых данных"), Space]
        [SerializeField]
        protected TRegistry dataRegistry;

        #region Получение связанных по id данных
        
        /// <summary>
        /// Получить хранимые в записи данные
        /// </summary>
        /// <param name="itemId">Идентификатор хранимых в записи данных</param>
        /// <returns>Данные записи</returns>
        public TItem GetItemDataById(string itemId)
        {
            if (dataRegistry == null)
            {
                ServiceDebug.LogError("Реестр данных не задан, данные не найдены");
                return null;
            }
            if (String.IsNullOrEmpty(itemId))
            {
                ServiceDebug.LogError("Невалидный id данных записи контейнера, данные не найдены");
                return null;
            }

            TItem foundData = dataRegistry.GetById(itemId);
            return foundData;
        }
        
        #endregion
    }
}