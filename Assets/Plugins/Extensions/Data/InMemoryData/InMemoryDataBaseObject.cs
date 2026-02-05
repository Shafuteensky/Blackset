using UnityEngine;

namespace Extensions.Data.InMemoryData
{
    /// <summary>
    /// Базовый класс InMemory хранилища данных
    /// </summary>
    public abstract class InMemoryDataBaseObject<TData> : ScriptableObject where TData : InMemoryDataItem { }
}