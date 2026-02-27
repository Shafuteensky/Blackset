using UnityEngine;

namespace Blackset.Storms
{
    /// <summary>
    /// Оверрайд правил дуэли
    /// </summary>
    /// <typeparam name="T">Тип данных правила</typeparam>
    [System.Serializable]
    public struct RuleOverride<T>
    {
        [SerializeField]
        private bool enabled;

        [SerializeField]
        private T value;

        /// <summary>
        /// Активен ли оверрайд
        /// </summary>
        public bool Enabled => enabled;
        /// <summary>
        /// Новое значение правила
        /// </summary>
        public T Value => value;

        /// <summary>
        /// Применить оверрайд к конфигурации
        /// </summary>
        /// <param name="target">Целевое правило конфигурации</param>
        public void Apply(ref T target)
        {
            if (enabled) target = value;
        }
    }
}