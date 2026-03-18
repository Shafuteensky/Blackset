using Blackset.Data.Base;
using UnityEngine;

namespace Blackset.Data.Items.Types
{
    /// <summary>
    /// База типа предмета
    /// </summary>
    public class InventoryItemType : BaseData
    {
        /// <summary>
        /// Иконка типа
        /// </summary>
        public Sprite Icon => icon;

        /// <summary>
        /// Множитель для мазовой цены расходника
        /// </summary>
        public float PriceMultiplier => priceMultiplier;
        
        [Header("Параметры репрезентации типа"), Space]
        [SerializeField]
        [Tooltip("Иконка типа")]
        private Sprite icon;
            
        [Header("Множитель цены")]
        [SerializeField]
        [Range(0f, 10f)]
        [Tooltip("Множитель для базовой цены расходника")]
        private float priceMultiplier;
    }
}