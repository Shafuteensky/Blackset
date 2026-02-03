using System;
using UnityEngine;

namespace Extensions.Data.InMemoryData.SelectionContext
{
    /// <summary>
    /// Хранилище идентификатора связанной с объектом единицы данных
    /// </summary>
    public class ContextIdHolder : MonoBehaviour
    {
        /// <summary>
        /// Событие инициализации
        /// </summary>
        public event Action onInitialized;

        /// <summary>
        /// Состояние инициализации
        /// </summary>
        public bool IsInitialized => _isInitialized;
        /// <summary>
        /// Связный идентификатор
        /// </summary>
        public string Id => _id;
        
        private bool _isInitialized = false;
        private string _id = string.Empty;
        
        /// <summary>
        /// Инициализация данных
        /// </summary>
        /// <param name="id">Идентификатор единицы данных</param>
        public void Initialize(string id)
        {
            _id = id;
            _isInitialized = true;
            onInitialized?.Invoke();
        }
    }
}