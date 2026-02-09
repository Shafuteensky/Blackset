using Blackset.Data.Base;
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

        [SerializeField]
        protected EffectConfig effect = default;
    }
}