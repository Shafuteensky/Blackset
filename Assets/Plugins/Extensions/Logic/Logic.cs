using Extensions.Log;
using UnityEngine;

namespace Extensions.Logic
{
    /// <summary>
    /// Базовая быстрая логика
    /// </summary>
    public static class Logic
    {
        #region Проверки
        
        /// <summary>
        /// Проверка объекта на существование
        /// </summary>
        /// <param name="obj">Проверяемый объект</param>
        /// <returns></returns>
        [HideInCallstack]
        public static bool IsNull(Object obj)
        {
            if (obj == null)
            {
#if UNITY_EDITOR
                ServiceDebug.LogError("Объект = NULL");
#endif
                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверка объекта на существование
        /// </summary>
        /// <param name="obj">Проверяемый объект</param>
        /// <returns></returns>
        [HideInCallstack]
        public static bool IsNotNull(Object obj) => !IsNull(obj);
        
        #endregion
    }
}