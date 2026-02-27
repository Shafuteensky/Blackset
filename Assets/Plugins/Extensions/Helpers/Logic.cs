using Extensions.Log;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extensions.Helpers
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
        public static bool IsNull(Object obj, string message = "")
        {
            if (obj == null)
            {
                ServiceDebug.LogError(message != "" ? message : "Объект = NULL");
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