using System;
using System.Collections.Generic;
using Blackset.Data.Base;
using UnityEngine;
using Extensions.Log;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Абстракция реестра игровых данных
    /// </summary>
    /// <typeparam name="TData">Тип данных</typeparam>
    public abstract class BaseDataRegistry<T> : BaseDataRegistry where T : BaseData
    {
        /// <summary>
        /// Данные реестра
        /// </summary>
        public IReadOnlyList<T> Data => data;
        
        [Header("Хранимые данные"), Space]
        [SerializeField]
        protected List<T> data = new List<T>();

        [NonSerialized]
        protected Dictionary<string, T> dataById;
        [NonSerialized]
        protected bool isIndexBuilt;

        /// <summary>
        /// Получить данные по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        public T GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ServiceDebug.LogError("Невалидный идентификатор, данные не получены");
                return null;
            }

            EnsureIndex();

            if (dataById == null)
            {
                ServiceDebug.LogError("Ошибка получения реестра записей");
                return null;
            }
            
            if (dataById.TryGetValue(id, out T result)) return result;
            
            ServiceDebug.LogError($"Данные с Id {id} не найдены в реестре записей");
            return null;
        }
        
        /// <summary>
        /// Получить случайную запись из реестра
        /// </summary>
        public virtual T GetRandom()
        {
            if (data == null || data.Count == 0)
            {
                ServiceDebug.LogError($"Реестр {name} пуст, случайная запись не получена");
                return null;
            }

            return data[UnityEngine.Random.Range(0, data.Count)];
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            isIndexBuilt = false;
            dataById = null;

            // Проверка на дубликаты идентификаторов и прочие editor-ошибки
#if UNITY_EDITOR
            if (data == null || data.Count == 0) return;

            HashSet<string> ids = null;

            for (int i = 0; i < data.Count; i++)
            {
                BaseData entry = data[i];
                if (entry == null)
                {
                    ServiceDebug.LogWarning($"[{name}] Элемент списка data (index {i}) = null");
                    continue;
                }

                string entryId = entry.Id;
                if (string.IsNullOrEmpty(entryId))
                {
                    ServiceDebug.LogError($"[{name}] Запись '{entry.name}' (index {i}) имеет пустой Id");
                    continue;
                }

                if (ids == null) ids = new HashSet<string>();

                if (!ids.Add(entryId))
                {
                    ServiceDebug.LogError($"[{name}] Дубликат Id '{entryId}' (index {i}). Реестр должен содержать уникальные Id");
                }
            }
#endif
        }

        private void EnsureIndex()
        {
            if (isIndexBuilt) return;

            isIndexBuilt = true;

            if (data == null || data.Count == 0)
            {
                dataById = null;
                return;
            }

            if (dataById == null)
            {
                dataById = new Dictionary<string, T>(data.Count);
            }
            else
            {
                dataById.Clear();
            }

            for (int i = 0; i < data.Count; i++)
            {
                T entry = data[i];
                if (entry == null) continue;

                string entryId = entry.Id;
                if (string.IsNullOrEmpty(entryId))
                {
                    ServiceDebug.LogWarning($"Запись без Id в {name} (index {i}) пропущена");
                    continue;
                }

                if (dataById.ContainsKey(entryId))
                {
                    ServiceDebug.LogError($"Дубликат Id '{entryId}' в {name}. Получен последний найденный, рекомендуется изменить идентификатор");
                }

                dataById[entryId] = entry;
            }
        }
    }
    
    /// <summary>
    /// Базовый класс реестра
    /// </summary>
    public abstract class BaseDataRegistry : BaseData { }
}
