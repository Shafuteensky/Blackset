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
        /// <summary>
        /// Цвет дайса // TODO заменить в будущем
        /// </summary>
        public Color Color => color;

        [SerializeField]
        protected EffectConfig effect;
        [SerializeField]
        protected Color color = Color.white;
    }
}