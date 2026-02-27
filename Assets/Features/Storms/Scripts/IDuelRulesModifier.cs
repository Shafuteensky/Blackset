using Blackset.Duel.Rules;

namespace Blackset.Storms
{
    /// <summary>
    /// Модификация конфигурации правил дуэли
    /// </summary>
    public interface IDuelRulesModifier
    {
        void Apply(ref DuelRulesConfiguration config);
    }
}