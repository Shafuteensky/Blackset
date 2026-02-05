using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Extensions.Log;
using Extensions.Identification;
using UnityEngine;

namespace Extensions.Data.InMemoryData
{
    /// <summary>
    /// Базовый класс хранимой и загружаемой модели in-memory таблицы данных
    /// <remarks>
    /// Загружается автоматически при первом доступе
    /// </remarks>
    /// </summary>
    /// <typeparam name="TData">Единица данных</typeparam>
    public abstract class InMemoryDataContainer<TData> : InMemoryDataBaseObject<List<TData>> where TData : InMemoryDataItem
    {
        protected const string FORMAT = "N";

        #region Events

        /// <summary>
        /// Событие добавления новой записи в таблицу
        /// </summary>
        public event Action onDataAdd;
        /// <summary>
        /// Событие удаления новой записи в таблице
        /// </summary>
        public event Action onDataRemove;
        /// <summary>
        /// Событие удаления всех записей таблицы 
        /// </summary>
        public event Action onDataClear;

        #endregion
        
        // Кэш-индекс данных для моментального доступа по идентификатору 
        protected Dictionary<string, TData> IndexById
        {
            get
            {
                EnsureLoaded();

                if (indexById == null || indexDirty)
                {
                    indexById = new Dictionary<string, TData>(data.Count);

                    for (int i = 0; i < data.Count; i++)
                    {
                        TData item = data[i];
                        if (item == null || string.IsNullOrEmpty(item.Id))
                        {
                            continue;
                        }

                        indexById[item.Id] = item;
                    }

                    indexDirty = false;
                }

                return indexById;
            }
        }
        protected Dictionary<string, TData> indexById;
        
        protected bool indexDirty;

        #region Get

        /// <summary>
        /// Попытка получения конкретной записи по идентификатору
        /// </summary>
        /// <param name="entryId">Идентификатор записи</param>
        /// <param name="entry">Найденная запись</param>
        /// <returns></returns>
        public bool TryGetById(string entryId, out TData entry)
        {
            entry = null;
            
            if (string.IsNullOrEmpty(entryId))
            {
                return false;
            }

            return IndexById.TryGetValue(entryId, out entry);
        }

        /// <summary>
        /// Получение конкретной записи по идентификатору
        /// </summary>
        /// <param name="entryId">Идентификатор записи</param>
        /// <returns></returns>
        public TData GetById(string entryId)
        {
            if (TryGetById(entryId, out TData entry))
            {
                return entry;
            }

            ServiceDebug.LogWarning($"Данные с id «{entryId}» не найдены в контейнере {name} ({nameof(TData)})");
            return null;
        }

        #endregion
    
        #region Add/Remove

        /// <summary>
        /// Добавить запись данных
        /// </summary>
        /// <param name="data">Данные записи для добавления</param>
        public void Add(TData data)
        {
            if (data == null)
            {
                ServiceDebug.LogWarning("Попытка добавить пустые данные, запись не добавлена");
                return;
            }
            
            EnsureLoaded();

            if (string.IsNullOrEmpty(data.Id))
            {
                data.Id = Guid.NewGuid().ToString(FORMAT);
            }

            this.data.Add(data);

            onDataAdd?.Invoke();
            MarkDirty();
        }

        /// <summary>
        /// Удалить запись данных по экземпляру
        /// </summary>
        /// <param name="entryData">Экземпляр записи данных для удаления</param>
        public void Remove(TData entryData)
        {
            EnsureLoaded();

            if (entryData == null || string.IsNullOrEmpty(entryData.Id))
            {
                ServiceDebug.LogWarning("Данные или его идентификатор пусты, запись не удалена");
                return;
            }

            if (data.Remove(entryData))
            {
                onDataRemove?.Invoke();
                MarkDirty();
            }
            else
                ServiceDebug.LogWarning($"Запись с id {entryData.Id} не найдена, запись не удалена");
        }

        /// <summary>
        /// Удалить запись данных по идентификатору
        /// </summary>
        /// <param name="entryId">Идентификатор записи данных для удаления</param>
        public void Remove(string entryId)
        {
            EnsureLoaded();

            if (string.IsNullOrEmpty(entryId))
            {
                ServiceDebug.LogWarning("Идентификатор пуст, запись не удалена");
                return;
            }

            for (int i = 0; i < data.Count; i++)
            {
                TData item = data[i];
                if (item != null && item.Id == entryId)
                {
                    data.RemoveAt(i);

                    onDataRemove?.Invoke();
                    MarkDirty();
                    return;
                }
            }

            ServiceDebug.LogWarning($"Запись с id {entryId} не найдена, запись не удалена");
        }

        /// <summary>
        /// Очистить все данные
        /// </summary>
        public void Clear()
        {
            EnsureLoaded();

            data.Clear();

            onDataClear?.Invoke();
            MarkDirty();
        }

        #endregion

        #region SaveLoad

        /// <summary>
        /// Запрос обновления записи списка 
        /// </summary>
        /// <param name="entryId">Идентификатор записи для обновления</param>
        /// <param name="newData">Новые данные записи</param>
        /// <returns></returns>
        public bool TryUpdate(string entryId, TData newData)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                ServiceDebug.LogWarning($"Идентификатор «{nameof(entryId)}» пуст, запись не обновлена");
                return false;
            }

            if (newData == null)
            {
                ServiceDebug.LogWarning($"Назначаемые данные отсутствуют, запись не обновлена");
                return false;
            }

            EnsureLoaded();

            newData.Id = entryId;

            for (int i = 0; i < data.Count; i++)
            {
                TData item = data[i];
                if (item != null && item.Id == entryId)
                {
                    data[i] = newData;

                    OnDataUpdate();
                    MarkDirty();
                    return true;
                }
            }

            ServiceDebug.LogWarning($"Запись с id «{entryId}» не найдена, запись не обновлена");
            return false;
        }
        
        protected override void MarkDirty()
        {
            indexDirty = true;
            
            base.MarkDirty();
        }
        
        #endregion
    }
}