using System;
using UnityEngine;

namespace Extensions.Data.InMemoryData.SelectionContext
{
    /// <summary>
    /// Контекст выбора InMemoryData-контейнера
    /// </summary>
    /// <typeparam name="TData">Тип хранимых данных</typeparam>
    // [CreateAssetMenu(fileName = nameof(SelectionContext), menuName = "Extensions/Data/InMemoryData/" + nameof(SelectionContext))]
    public abstract class SelectionContext<TData> : BaseSelectionContext where TData : InMemoryDataItem
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
        public string SelectedId { get; private set; }

        public override bool HasSelection => Container != null && !string.IsNullOrEmpty(SelectedId);

        public override void Clear()
        {
            Container = null;
            SelectedId = string.Empty;

            onSelectionChanged?.Invoke();
        }
        
        /// <summary>
        /// Выбрать данные как активные
        /// </summary>
        /// <param name="container">Конейнер данных</param>
        /// <param name="dataItemId">Идентификатор</param>
        public void Select(InMemoryDataContainer<TData> container, string dataItemId)
        {
            Container = container;
            SelectedId = dataItemId;

            onSelectionChanged?.Invoke();
        }

        /// <summary>
        /// Получить активные данные
        /// </summary>
        /// <param name="dataItem"></param>
        /// <returns></returns>
        public bool TryGetSelected(out TData dataItem)
        {
            dataItem = null;

            if (!HasSelection)
            {
                return false;
            }

            return Container.TryGetById(SelectedId, out dataItem);
        }
    }
}