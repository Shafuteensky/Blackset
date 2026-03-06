using Extensions.Log;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Базовый модуль обработчиков данных дуэли
    /// </summary>
    public abstract class BaseDuelModule : ScriptableObject
    {
        protected bool isInitialized = false;
        
        /// <summary>
        /// Состояние инициализации модуля
        /// </summary>
        public bool IsInitialized()
        {
            if (isInitialized) return true;
            ServiceDebug.LogError("Модуль не инициализирован");
            return false;
        }
    }
}