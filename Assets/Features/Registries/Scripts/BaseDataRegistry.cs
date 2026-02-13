using System;
using System.Collections.Generic;
using Blackset.Data.Base;
using UnityEngine;
using Extensions.Log;
using Random = UnityEngine.Random;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Базовый реестр игровых данных
    /// </summary>
    /// <typeparam name="TData">Тип данных</typeparam>
    public abstract class BaseDataRegistry<TData> : BaseData where TData : BaseData
    {
        /// <summary>
        /// Данные реестра
        /// </summary>
        public IReadOnlyList<TData> Data => data;
        
        [SerializeField]
        protected List<TData> data = new List<TData>();

        [NonSerialized]
        protected Dictionary<string, TData> dataById;
        [NonSerialized]
        protected bool isIndexBuilt;

        /// <summary>
        /// Получить данные по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        public TData GetById(string id)
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
            
            if (dataById.TryGetValue(id, out TData result)) return result;
            
            ServiceDebug.LogError($"Данные с Id {id} не найдены в реестре записей");
            return null;
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
                TData entry = data[i];
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
                dataById = new Dictionary<string, TData>(data.Count);
            }
            else
            {
                dataById.Clear();
            }

            for (int i = 0; i < data.Count; i++)
            {
                TData entry = data[i];
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
}
