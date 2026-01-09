using System;
using System.Collections.Generic;
using Extensions.Logs;
using UnityEngine;

namespace Extensions.Data.InMemoryData
{
    using ID;
    
    /// <summary>
    /// Базовый класс хранимой и загружаемой модели in-memory БД
    /// <remarks>
    /// Загружается автоматически при первом доступе
    /// </remarks>
    /// </summary>
    /// <typeparam name="TData">Единица данных</typeparam>
    public abstract class InMemoryDataBase<TData> : InMemoryDataBaseObject where TData : InMemoryDataEntry
    {
        protected const string FORMAT = "N";
        
        [SerializeField]
        protected string saveFileName = string.Empty;
        
        [SerializeField]
        protected ID id = default;

        [SerializeField]
        protected bool autoSave = true;

        #region Events

        /// <summary>
        /// Событие добавления новой записи в БД
        /// </summary>
        public event Action OnDataAdd;
        /// <summary>
        /// Событие удаления новой записи в БД
        /// </summary>
        public event Action OnDataRemove;
        /// <summary>
        /// Событие удаления всех записей БД
        /// </summary>
        public event Action OnDataClear;
        /// <summary>
        /// Событие обновления БД
        /// </summary>
        public event Action OnDataUpdate;
        /// <summary>
        /// Событие загрузки БД
        /// </summary>
        public event Action OnDataLoaded;
        /// <summary>
        /// Событие сохранения БД
        /// </summary>
        public event Action OnDataSaved;
        /// <summary>
        /// Событие ошибки сохранения БД
        /// </summary>
        public event Action OnDataSaveError;

        #endregion

        /// <summary>
        /// Данные
        /// </summary>
        public IReadOnlyList<TData> DataBase
        {
            get
            {
                EnsureLoaded();
                return data;
            }
        }

        protected List<TData> data;
        
        protected bool loaded;
        protected bool dirty;

        protected virtual void OnEnable()
        {
            if (string.IsNullOrEmpty(saveFileName))
            {
                saveFileName = GetType().Name;
            }
        }

        #region DataChange

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
                data.Id = Guid.NewGuid().ToString("N");
            }

            this.data.Add(data);

            OnDataAdd?.Invoke();
            MarkDirty();
        }

        /// <summary>
        /// Удалить запись данных по экземпляру
        /// </summary>
        /// <param name="data">Экземпляр записи данных для удаления</param>
        public void Remove(TData data)
        {
            EnsureLoaded();

            if (data == null || string.IsNullOrEmpty(data.Id))
            {
                ServiceDebug.LogWarning("Данные или его идентификатор пусты, запись не удалена");
                return;
            }

            if (this.data.Remove(data))
            {
                OnDataRemove?.Invoke();
                MarkDirty();
            }
            else
                ServiceDebug.LogWarning($"Запись с id {data.Id} не найдена, запись не удалена");
        }

        /// <summary>
        /// Удалить запись данных по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор записи данных для удаления</param>
        public void Remove(string id)
        {
            EnsureLoaded();

            if (string.IsNullOrEmpty(id))
            {
                ServiceDebug.LogWarning("Идентификатор пуст, запись не удалена");
                return;
            }

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Id == id)
                {
                    data.RemoveAt(i);

                    OnDataRemove?.Invoke();
                    MarkDirty();
                    return;
                }
            }

            ServiceDebug.LogWarning($"Запись с id {id} не найдена, запись не удалена");
        }


        /// <summary>
        /// Очистить все данные
        /// </summary>
        public void Clear()
        {
            EnsureLoaded();

            data.Clear();

            OnDataUpdate?.Invoke();
            OnDataClear?.Invoke();
            Save();
        }

        #endregion

        #region SaveLoad

        /// <summary>
        /// Запрос сохранения БД
        /// </summary>
        public void RequestSave()
        {
            EnsureLoaded();
            Save();
        }

        protected void Save()
        {
            if (!loaded || !dirty)
            {
                return;
            }

            if (JsonSaveLoad.Save(data, saveFileName))
            {
                dirty = false;
                OnDataSaved?.Invoke();
            }
            else
                OnDataSaveError?.Invoke();
        }
        
        protected void EnsureLoaded()
        {
            if (loaded)
            {
                return;
            }

            data = JsonSaveLoad.Load(saveFileName, new List<TData>()) ?? new List<TData>();

            loaded = true;
            OnDataLoaded?.Invoke();
        }

        private void MarkDirty()
        {
            dirty = true;
            OnDataUpdate?.Invoke();

            if (autoSave)
            {
                Save();
            }
        }

        #endregion
    }
}
