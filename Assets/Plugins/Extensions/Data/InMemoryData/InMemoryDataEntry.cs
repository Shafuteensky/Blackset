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
        protected const string GUID_FORMAT = "N";
        
        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id => id;
        
        private string id = Guid.NewGuid().ToString(GUID_FORMAT);
    }
}