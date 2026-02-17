using Blackset.Data.Base;
using UnityEngine;

namespace Blackset.Data.Items.Types
{
    /// <summary>
    /// База типа предмета
    /// </summary>
    public abstract class BaseItemType : BaseData
    {
        /// <summary>
        /// Иконка типа
        /// </summary>
        public Sprite Icon => icon;

        [Header("Параметры репрезентации типа"), Space]
        
        [SerializeField]
        [Tooltip("Иконка типа")]
        private Sprite icon;
    }
}