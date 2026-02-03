using System;
using System.Collections.Generic;
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
    public abstract class InMemoryDataContainer<TData> : InMemoryDataBaseObject where TData : InMemoryDataItem
    {
        protected const string FORMAT = "N";
        
        [SerializeField]
        protected string saveFileName = string.Empty;
        
        [SerializeField]
        protected ID id = default;

        [SerializeField]
        protected bool autoSave = true;
        
        /// <summary>
        /// Название файла сохранения
        /// </summary>
        public string SaveFileName => saveFileName;
        
        /// <summary>
        /// Процесс выполнения сохранения
        /// </summary>
        public bool IsSavingInProgress { get; private set; }

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
        /// <summary>
        /// Событие обновления данных таблицы
        /// </summary>
        public event Action onDataUpdate;
        /// <summary>
        /// Событие измнения данных таблицы
        /// </summary>
        public event Action onDataChange;
        /// <summary>
        /// Событие загрузки таблицы 
        /// </summary>
        public event Action onDataLoaded;
        /// <summary>
        /// Событие сохранения таблицы
        /// </summary>
        public event Action onDataSaved;
        /// <summary>
        /// Событие ошибки сохранения таблицы 
        /// </summary>
        public event Action onDataSaveError;

        #endregion

        /// <summary>
        /// Данные
        /// </summary>
        public IReadOnlyList<TData> Data
        {
            get
            {
                EnsureLoaded();
                return data;
            }
        }
        
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

        protected List<TData> data;
        protected Dictionary<string, TData> indexById;
        
        protected bool loaded;
        protected bool dirty;
        protected bool indexDirty;

        // Назнаает имя файла сохранения при создании нового скриптового файла таблицы
        protected virtual void OnEnable()
        {
            if (string.IsNullOrEmpty(saveFileName))
            {
                saveFileName = GetType().Name;
            }
        }

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
        /// Запрос обновления таблицы 
        /// </summary>
        /// <param name="entryId">Идентификатор записи для обновления</param>
        /// <param name="newData">Новые данные записи</param>
        /// <returns></returns>
        public bool RequestUpdate(string entryId, TData newData)
        {
            EnsureLoaded();

            if (string.IsNullOrEmpty(entryId))
            {
                ServiceDebug.LogWarning($"Идентификатор {nameof(entryId)} пуст, запись не обновлена");
                return false;
            }

            if (newData == null)
            {
                ServiceDebug.LogWarning($"Данные {nameof(newData)} пусты, запись не обновлена");
                return false;
            }

            newData.Id = entryId;

            for (int i = 0; i < data.Count; i++)
            {
                TData item = data[i];
                if (item != null && item.Id == entryId)
                {
                    data[i] = newData;

                    onDataUpdate?.Invoke();
                    MarkDirty();
                    return true;
                }
            }

            ServiceDebug.LogWarning($"Запись с id {entryId} не найдена, запись не обновлена");
            return false;
        }
    
        /// <summary>
        /// Запрос сохранения таблицы
        /// </summary>
        /// <returns>Сохранена ли таблица</returns>
        public bool RequestSave()
        {
            EnsureLoaded();
            return Save();
        }

        protected bool Save()
        {
            if (!loaded)
            {
                ServiceDebug.LogWarning($"Попытка сохранения еще не загруженной таблицы {name}");
                return false;
            }
            
            if (!dirty)
            {
                return false;
            }

            if (JsonSaveLoad.Save(data, saveFileName))
            {
                dirty = false;
                onDataSaved?.Invoke();
            }
            else
            {
                onDataSaveError?.Invoke();
                return false;
            }
            
            return true;
        }
        
        protected void EnsureLoaded()
        {
            if (loaded)
            {
                return;
            }

            data = JsonSaveLoad.Load(saveFileName, new List<TData>()) ?? new List<TData>();

            loaded = true;
            onDataLoaded?.Invoke();
        }

        protected void MarkDirty()
        {
            dirty = true;
            indexDirty = true;
            
            onDataChange?.Invoke();

            if (autoSave)
            {
                Save();
            }
        }

        #endregion
    }
}
