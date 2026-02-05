using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Extensions.Log;
using UnityEngine;

namespace Extensions.Data.InMemoryData
{
    /// <summary>
    /// Сохранение InMemory БД на OnDisable
    /// <remarks>
    /// Используется для сохранения данных БД с отключенным автосейвом
    /// Поддерживает как синхронное, так и асинхронное сохранение
    /// </remarks>
    /// </summary>
    public class InMemoryDataDisableSaver : MonoBehaviour
    {
        [SerializeField]
        protected List<InMemoryDataBaseObject> dataBases = new List<InMemoryDataBaseObject>();

        [SerializeField]
        [Tooltip("Ждать завершения сохранения перед отключением (рекомендуется)")]
        protected bool waitForSaveCompletion = true;

        [SerializeField]
        [Tooltip("Показывать лог сохранения")]
        protected bool showSaveLog = false;

        protected virtual async void OnDisable()
        {
            if (waitForSaveCompletion)
            {
                await SaveAllAsync();
            }
            else
            {
                SaveAllSync();
            }
        }

        /// <summary>
        /// Асинхронное сохранение всех БД (ждет завершения)
        /// </summary>
        private async UniTask SaveAllAsync()
        {
            if (showSaveLog)
            {
                ServiceDebug.Log($"[{name}] Начало сохранения {dataBases.Count} БД...");
            }

            int savedCount = 0;
            foreach (InMemoryDataBaseObject dataBase in dataBases)
            {
                if (dataBase == null) continue;

                if (dataBase is InMemoryDataContainer<InMemoryDataItem> inMemoryDataBase)
                {
                    bool success = await inMemoryDataBase.RequestSaveAsync();
                    
                    if (success)
                    {
                        savedCount++;
                        if (showSaveLog)
                        {
                            ServiceDebug.Log($"[{name}] Сохранена БД: {dataBase.name}");
                        }
                    }
                }
            }

            if (showSaveLog)
            {
                ServiceDebug.Log($"[{name}] Сохранено {savedCount}/{dataBases.Count} БД");
            }
        }

        /// <summary>
        /// Синхронное сохранение (fire-and-forget)
        /// </summary>
        private void SaveAllSync()
        {
            if (showSaveLog)
            {
                ServiceDebug.Log($"[{name}] Быстрое сохранение {dataBases.Count} БД (fire-and-forget)...");
            }

            foreach (InMemoryDataBaseObject dataBase in dataBases)
            {
                if (dataBase == null) continue;

                if (dataBase is InMemoryDataContainer<InMemoryDataItem> inMemoryDataBase)
                {
                    inMemoryDataBase.RequestSave();
                }
            }
        }

        /// <summary>
        /// Ручное сохранение всех БД извне (асинхронное)
        /// </summary>
        public async UniTask<int> SaveAllManualAsync()
        {
            int savedCount = 0;
            foreach (InMemoryDataBaseObject dataBase in dataBases)
            {
                if (dataBase == null) continue;

                if (dataBase is InMemoryDataContainer<InMemoryDataItem> inMemoryDataBase)
                {
                    if (await inMemoryDataBase.RequestSaveAsync())
                    {
                        savedCount++;
                    }
                }
            }
            return savedCount;
        }

        /// <summary>
        /// Ручное сохранение всех БД извне (синхронное fire-and-forget)
        /// </summary>
        public void SaveAllManual() => SaveAllSync();
    }
}