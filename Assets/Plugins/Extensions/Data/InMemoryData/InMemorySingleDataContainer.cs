using Extensions.Log;

namespace Extensions.Data.InMemoryData
{
    /// <summary>
    /// Базовый класс хранимой и загружаемой in-memory одиночной модели данных
    /// <remarks>
    /// Загружается автоматически при первом доступе
    /// </remarks>
    /// </summary>
    /// <typeparam name="TData">Тип единицы данных</typeparam>
    public abstract class InMemorySingleDataContainer<TData> : InMemoryDataBaseObject<TData> where TData : new()
    {
        /// <summary>
        /// Обновить данные
        /// </summary>
        /// <param name="newData"></param>
        /// <returns></returns>
        public void Set(TData newData)
        {
            if (newData == null)
            {
                ServiceDebug.LogWarning($"Назначаемые данные отсутствуют, данные не обновлены");
                return;
            }
            
            EnsureLoaded();
            
            data = newData;

            NotifyUpdated();
        }

        /// <summary>
        /// Ручное оповещение о измененности данных
        /// </summary>
        /// <remarks>
        /// Например для подхвата обновленных данных при изменении значений полей экземпляра хранимого класса
        /// </remarks>
        public void NotifyUpdated()
        {
            EnsureLoaded();
            
            MarkDirty(); 
            OnDataUpdated();
        }
    }
}