using UnityEngine;

namespace Extensions.Data.InMemoryData.SelectionContext
{
    /// <summary>String
    /// Базовый вспомогательный класс контекста выбора InMemoryData-контейнера
    /// </summary>
    public abstract class BaseSelectionContext : ScriptableObject
    {
        /// <summary>
        /// Наличие активного выбранного элемента
        /// </summary>
        public abstract bool HasSelection { get; }
        
        /// <summary>
        /// Очистка выбора
        /// </summary>
        public abstract void Clear();
    }
}