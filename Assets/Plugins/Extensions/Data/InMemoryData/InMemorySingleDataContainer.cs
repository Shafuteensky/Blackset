using System.Collections.Generic;

namespace Extensions.Data.InMemoryData
{
    /// <summary>
    /// Базовый класс хранимой и загружаемой in-memory одиночной модели данных
    /// <remarks>
    /// Загружается автоматически при первом доступе
    /// </remarks>
    /// </summary>
    /// <typeparam name="TData">Тип единицы данных</typeparam>
    public abstract class InMemorySingleDataContainer<TData> : InMemoryDataBaseObject<TData>
        where TData : InMemoryDataItem
    {
        protected const string SINGLE_ID = "single";

        /// <summary>
        /// Данные (одиночная запись)
        /// </summary>
        // public TData Value
        // {
        //     get
        //     {
        //         //
        //     }
        // }
    }
}