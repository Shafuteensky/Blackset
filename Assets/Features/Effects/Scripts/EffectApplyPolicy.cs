namespace Blackset.Effects
{
    /// <summary>
    /// Политика применения эффекта
    /// </summary>
    public enum EffectApplyPolicy
    {
        /// <summary>
        /// Единожды при использовании во время фазы примененяи эффекта
        /// </summary>
        OnUse,
        /// <summary>
        /// Единожды на следующий бросок во время фазы примененяи эффекта
        /// </summary>
        OnNextThrow,
        /// <summary>
        /// Каждый бросок во время фазы применения эффекта
        /// </summary>
        EveryThrowWhileUsed,
        /// <summary>
        /// Единожды за битву во время фазы примененяи эффекта
        /// </summary>
        OncePerBattle
    }
}