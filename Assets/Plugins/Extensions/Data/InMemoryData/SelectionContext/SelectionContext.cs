using System;
using Extensions.Log;
using UnityEngine;

namespace Extensions.Data.InMemoryData.SelectionContext
{
    /// <summary>
    /// Контекст выбора InMemoryData-контейнера
    /// </summary>
    /// <typeparam name="TData">Тип хранимых данных</typeparam>
    public abstract class SelectionContext<TData> : BaseSelectionContext where TData : InMemoryDataEntry
    {
        /// <summary>
        /// Событие изменения активного элемента
        /// </summary>
        public event Action onSelectionChanged;

        /// <summary>
        /// Контейнер, данные которого назначаются как активный выбор контекста
        /// </summary>
        [field: SerializeField]
        public InMemoryDataContainer<TData> Container { get; private set; }

        /// <summary>
        /// Идентификатор активной выбранной записи контейнера
        /// </summary>
        public string SelectedId => selectedIdStatic;

        public override bool HasSelection => IsContainerInited() && !string.IsNullOrEmpty(SelectedId);

        private static string selectedIdStatic;

        /// <summary>
        /// Выбрать данные как активные
        /// </summary>
        /// <param name="dataItemId">Идентификатор</param>
        public void Select(string dataItemId)
        {
            selectedIdStatic = dataItemId;
            onSelectionChanged?.Invoke();
        }

        /// <summary>
        /// Очистить выбор
        /// </summary>
        public override void Clear()
        {
            selectedIdStatic = string.Empty;
            onSelectionChanged?.Invoke();
        }

        #region Получение данных

        /// <summary>
        /// Получить активные данные
        /// </summary>
        /// <returns>Выбранные контекстом данные</returns>
        public TData GetSelectedData()
        {
            if (!HasSelection) return null;

            Container.GetById(SelectedId, out TData dataItem);
            return dataItem;
        }

        /// <summary>
        /// Попытка получить активные данные
        /// </summary>
        /// <param name="dataItem">out: выбранные контекстом данные</param>
        /// <returns>true если данные в контейнере успешно найдены, иначе false</returns>
        public bool TryGetSelectedData(out TData dataItem)
        {
            dataItem = null;

            if (!HasSelection || !IsContainerInited()) return false;

            return Container.GetById(SelectedId, out dataItem);
        }

        #endregion

        private bool IsContainerInited()
        {
            if (Container == null)
            {
                ServiceDebug.LogError("Контейнер не назначен, данные невозможно получить");
                return false;
            }

            return true;
        }
    }
}