namespace Blackset.Effects
{
    /// <summary>
    /// Фазы обработки эффектов
    /// </summary>
    public enum EffectPhase
    {
        DiceConfiguration,
        PreRoll,
        OnRoll,
        PostRoll,
        PreApply,
        OnApply,
        PostApply
    }
}