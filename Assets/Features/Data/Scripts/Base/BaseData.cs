using Extensions.Identification;
using UnityEngine;

namespace Blackset.Data.Base
{
    /// <summary>
    /// Базовая единица игровых данных
    /// </summary>
    public abstract class BaseData : IdentifiableObject
    {
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string DataName => dataName; // TODO: Заменить на получение строки локализации

        [SerializeField]
        protected string dataName = string.Empty;
    }
}