using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Extensions.Log;
using Extensions.Identification;
using UnityEngine;

namespace Extensions.Data.InMemoryData
{
    /// <summary>
    /// Базовый класс хранимой и загружаемой модели in-memory хранилища данных
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
        /// Событие добавления новой записи в хранилище
        /// </summary>
        /// <param name="int">Индекс добавленной записи</param>
        /// <param name="TData">Данные добавленной записи</param>
        public event Action<int, TData> onEntryAdded;
        /// <summary>
        /// Событие удаления новой записи в хранилище
        /// </summary>
        /// <param name="int">Индекс добавленной записи</param>
        /// <param name="TData">Данные добавленной записи</param>
        public event Action onEntryRemoved;
        
        /// <summary>
        /// Событие удаления всех записей хранилища 
        /// </summary>
        public event Action onDataCleared;
        /// <summary>
        /// Событие любого обновления содержимого хранилища 
        /// </summary>
        public event Action onDataUpdated;

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

                    foreach (var item in data)
                    {
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
        /// <returns>true если запись найдена, иначе false</returns>
        public bool GetById(string entryId, out TData entry)
        {
            entry = null;
            
            if (string.IsNullOrEmpty(entryId))
            {
                ServiceDebug.LogError("Невалидный id, данные не найдены");
                return false;
            }

            return IndexById.TryGetValue(entryId, out entry);
        }

        /// <summary>
        /// Получение конкретной записи по идентификатору
        /// </summary>
        /// <param name="entryId">Идентификатор записи</param>
        /// <returns>Найденная запись или null</returns>
        public TData GetById(string entryId)
        {
            if (GetById(entryId, out TData entry))
            {
                return entry;
            }

            ServiceDebug.LogWarning($"Данные с id «{entryId}» не найдены в контейнере {name} ({nameof(TData)})");
            return null;
        }

        /// <summary>
        /// Попытка получения конкретной записи по индексу
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="entry">Найденная запись</param>
        /// <returns>true если запись найдена, иначе false</returns>
        public bool GetByIndex(int index, out TData entry)
        {
            entry = null;
            
            if (index < 0 || index >= data.Count)
            {
                ServiceDebug.LogError("Невалидный индекс, данные не найдены");
                return false;
            }

            entry = data[index];
            return true;
        }

        /// <summary>
        /// Получение конкретной записи по индексу
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="entry">Найденная запись</param>
        /// <returns>Найденная запись или null</returns>
        public TData GetByIndex(int index)
        {
            if (index < 0 || index >= data.Count)
            {
                ServiceDebug.LogError("Невалидный индекс, данные не найдены");
                return null;
            }

            return data[index];
        }

        #endregion
    
        #region Add/Remove

        /// <summary>
        /// Добавить запись данных
        /// </summary>
        /// <param name="data">Данные записи для добавления</param>
        /// <returns>true если добавление успешно, иначе false</returns>
        public bool Add(TData entry) // TODO Рассмотреть Insert и сортировку
        {
            if (entry == null)
            {
                ServiceDebug.LogWarning("Попытка добавить пустые данные, запись не добавлена");
                return false;
            }
            
            EnsureLoaded();

            data.Add(entry);

            int index = data.Count - 1;
            onEntryAdded?.Invoke(index, entry);
            onDataUpdated?.Invoke();
            MarkDirty();
            return true;
        }

        /// <summary>
        /// Удалить запись данных по экземпляру
        /// </summary>
        /// <param name="entry">Экземпляр записи данных для удаления</param>
        /// <returns>true если удаление успешно, иначе false</returns>
        public bool Remove(TData entry)
        {
            EnsureLoaded();

            if (entry == null || string.IsNullOrEmpty(entry.Id))
            {
                ServiceDebug.LogWarning("Данные или его идентификатор пусты, запись не удалена");
                return false;
            }

            if (data.Remove(entry))
            {
                onEntryRemoved?.Invoke();
                onDataUpdated?.Invoke();
                MarkDirty();
                return true;
            }
            
            ServiceDebug.LogWarning($"Запись с id {entry.Id} не найдена, запись не удалена");
            return false;
        }

        /// <summary>
        /// Удалить запись данных по идентификатору
        /// </summary>
        /// <param name="entryId">Идентификатор записи данных для удаления</param>
        /// <returns>true если удаление успешно, иначе false</returns>
        public bool Remove(string entryId)
        {
            EnsureLoaded();

            if (string.IsNullOrEmpty(entryId))
            {
                ServiceDebug.LogWarning("Идентификатор пуст, запись не удалена");
                return false;
            }

            for (int i = 0; i < data.Count; i++)
            {
                TData item = data[i];
                if (item != null && item.Id == entryId)
                {
                    Remove(i);
                    return true;
                }
            }

            ServiceDebug.LogWarning($"Запись с id {entryId} не найдена, запись не удалена");
            return false;
        }

        /// <summary>
        /// Удалить запись данных по индексу
        /// </summary>
        /// <param name="index">Индекс записи данных для удаления</param>
        /// <returns>true если удаление успешно, иначе false</returns>
        public bool Remove(int index)
        {
            if (index < 0 || index >= data.Count)
            {
                return false;
            }

            data.RemoveAt(index);

            onEntryRemoved?.Invoke();
            onDataUpdated?.Invoke();
            MarkDirty();

            return true;
        }

        /// <summary>
        /// Очистить все данные
        /// </summary>
        public void Clear()
        {
            EnsureLoaded();

            data.Clear();

            onDataCleared?.Invoke();
            onDataUpdated?.Invoke();
            MarkDirty();
        }

        #endregion

        #region SaveLoad
        
        protected override void MarkDirty()
        {
            indexDirty = true;
            
            base.MarkDirty();
        }
        
        #endregion
    }
}