using System;

namespace Extensions.Data.InMemoryData
{
    /// <summary>
    /// Базовый класс единицы данных
    /// <remarks>
    /// Дополняется полями данных
    /// </remarks>
    /// </summary>
    [Serializable]
    public abstract class InMemoryDataEntry
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id = String.Empty;
    }
}