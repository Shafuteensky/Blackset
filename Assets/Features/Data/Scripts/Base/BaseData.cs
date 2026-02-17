using Extensions.Identification;
using UnityEngine;

namespace Blackset.Data.Base
{
    /// <summary>
    /// Базовая единица игровых данных
    /// </summary>
    public abstract class BaseData : IdentifiableObject
    {
        // TODO: Заменить строки на получение строк локализации
        /// <summary>
        /// Наименование
        /// </summary>
        public string DataName => dataName; 
        /// <summary>
        /// Описание
        /// </summary>
        public string DataDescription => dataDescription;

        [Header("Название и описание"), Space]
        [SerializeField]
        protected string dataName = string.Empty;
        [SerializeField]
        protected string dataDescription = string.Empty;
    }
}