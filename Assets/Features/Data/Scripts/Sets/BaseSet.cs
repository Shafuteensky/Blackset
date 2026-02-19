using Blackset.Data.Base;
using UnityEngine;

namespace Blackset.Data
{
    /// <summary>
    /// Базовый набор игровых единиц (дайсы/расходники)
    /// </summary>
    public abstract class BaseSet : BaseData
    {
        /// <summary>
        /// Цвет набора
        /// </summary>
        public Color Color => color;
        
        [Header("Набор"), Space]
        [SerializeField]
        protected Color color;
    }
}