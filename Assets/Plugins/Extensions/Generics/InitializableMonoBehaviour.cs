using System;
using UnityEngine;

namespace Extensions.Generics
{
    /// <summary>
    /// Инициализируемый MonoBehaviour
    /// </summary>
    public class InitializableMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Событие инициализации
        /// </summary>
        public event Action onInitialized;
        
        /// <summary>
        /// Состояние инициализации
        /// </summary>
        public bool IsInitialized => _isInitialized;
        
        private bool _isInitialized = false;

        /// <summary>
        /// Инициализация
        /// </summary>
        public virtual void Initialize()
        {
            _isInitialized = true;
            onInitialized?.Invoke();
        }
    }
}