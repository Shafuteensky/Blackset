using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Базовые данные игровой единицы, содержащей эффект
    /// </summary>
    public abstract class EffectingItemData : BaseData
    {
        /// <summary>
        /// Конфигурация эффекта
        /// </summary>
        public EffectConfig Effect => effect;
        /// <summary>
        /// Цвет предмета // TODO заменить в будущем
        /// </summary>
        public Color Color => color;
        /// <summary>
        /// Набор, к которому относится предмет
        /// </summary>
        public BaseSet Set => set;
        /// <summary>
        /// Доступные типы предметов для этих данных
        /// </summary>
        public List<BaseItemType> AvailableTypes => availableTypes;

        [Header("Особенности"), Space]
        [SerializeField]
        protected EffectConfig effect;
        [SerializeField]
        protected Color color = Color.white;
        [SerializeField]
        private BaseSet set;
        
        [Header("Типы")]
        [SerializeField]
        private List<BaseItemType> availableTypes = new List<BaseItemType>();
    }
}