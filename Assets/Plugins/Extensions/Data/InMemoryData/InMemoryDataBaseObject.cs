using System;
using Cysharp.Threading.Tasks;
using Extensions.Identification;
using Extensions.Log;
using UnityEngine;

namespace Extensions.Data.InMemoryData
{
    /// <summary>
    /// Базовый класс InMemory хранилища данных
    /// </summary>
    public abstract class InMemoryDataBaseObject<TData> : InMemoryDataBaseObject where TData : new()
    {
        #region Events
        
        /// <summary>
        /// Событие загрузки данных 
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
        /// <summary>
        /// Событие изменения данных таблицы
        /// </summary>
        public event Action onDataChange;
        
        /// <summary>
        /// Событие обновления данных таблицы
        /// </summary>
        public event Action onDataUpdate;
        
        #endregion
        
        [SerializeField]
        protected ID id = default;

        [SerializeField]
        protected string saveKey = string.Empty;
        
        [SerializeField]
        protected bool autoSave = true;
        
        /// <summary>
        /// Название файла сохранения
        /// </summary>
        public string SaveKey => saveKey;

        /// <summary>
        /// Хранимые данные
        /// </summary>
        public TData Data
        {
            get
            {
                EnsureLoaded();
                return data;
            }
        }

        [NonSerialized]
        protected TData data;
        
        [NonSerialized]
        protected bool loaded = false;
        [NonSerialized]
        protected bool dirty;
        
        protected virtual void OnEnable()
        {
            // Назначает имя файла сохранения при создании нового скриптового файла таблицы
            if (string.IsNullOrEmpty(saveKey))
            {
                saveKey = GetType().Name;
            }
        }
        
        /// <summary>
        /// Гарантированная загрузка данных (синхронно через кэш)
        /// </summary>
        public override void EnsureLoaded()
        {
            if (loaded)
            {
                return;
            }
    
            // Синхронная загрузка через кэш JsonSaveLoad
            data = JsonSaveLoad.Load(saveKey, new TData());
            if (data == null)
            {
                ServiceDebug.LogWarning($"Данные контейнера {name} пусты, загружены данные по-умолчанию");
                data = new TData();
            }

            loaded = true;
            onDataLoaded?.Invoke();
        }

        #region  Save/Load

        /// <summary>
        /// Запрос сохранения таблицы (синхронный)
        /// </summary>
        /// <returns>Сохранена ли таблица</returns>
        public override bool RequestSave()
        {
            EnsureLoaded();
            return Save();
        }

        /// <summary>
        /// Запрос сохранения данных (асинхронный)
        /// </summary>
        /// <returns>Сохранены ли данные</returns>
        public override async UniTask<bool> RequestSaveAsync()
        {
            EnsureLoaded();
            return await SaveAsync();
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

            // Синхронное сохранение через кэш
            if (JsonSaveLoad.Save(data, saveKey))
            {
                dirty = false;
                onDataSaved?.Invoke();
                return true;
            }
            else
            {
                onDataSaveError?.Invoke();
                return false;
            }
        }

        protected async UniTask<bool> SaveAsync()
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

            if (await JsonSaveLoad.SaveAsync(data, saveKey))
            {
                dirty = false;
                onDataSaved?.Invoke();
                return true;
            }
            else
            {
                onDataSaveError?.Invoke();
                return false;
            }
        }

        /// <summary>
        /// Предзагрузка данных асинхронно
        /// </summary>
        public override async UniTask PreloadAsync()
        {
            if (loaded)
            {
                return;
            }

            await JsonSaveLoad.PreloadAsync(saveKey, new TData());
            
            // После preload данные уже в кэше, можем загрузить синхронно
            EnsureLoaded();
        }

        protected virtual void MarkDirty()
        {
            dirty = true;
            
            onDataChange?.Invoke();

            if (autoSave)
            {
                Save();
            }
        }

        #endregion
        
        protected void OnDataUpdate() => onDataUpdate?.Invoke();
    }

    /// <summary>
    /// Абстракция
    /// </summary>
    public abstract class InMemoryDataBaseObject : ScriptableObject
    {
        public abstract bool RequestSave();
        public abstract UniTask  PreloadAsync();
        public abstract UniTask<bool> RequestSaveAsync();
        public abstract void EnsureLoaded();
    }
}