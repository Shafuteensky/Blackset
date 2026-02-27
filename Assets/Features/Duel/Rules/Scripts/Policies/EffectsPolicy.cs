namespace Blackset.Duel.Rules
{
    /// <summary>
    /// Политика применения эффектов
    /// </summary>
    public enum EffectsPolicy
    {
        Both, // Эффекты применяются в обе стороны от обоих участников
        SelfOnly, // Эффекты применяются только на себя
        NoEffects // Никаких эффектов ни для кого
    }
}