using Blackset.Duel.Rules;

namespace Blackset.Storms
{
    /// <summary>
    /// Модификация конфигурации правил дуэли
    /// </summary>
    public interface IDuelRulesModifier
    {
        /// <summary>
        /// Применение модификатора к конфигурации
        /// </summary>
        /// <param name="config">Конфигурация</param>
        void Apply(ref DuelRulesConfiguration config);
    }
}