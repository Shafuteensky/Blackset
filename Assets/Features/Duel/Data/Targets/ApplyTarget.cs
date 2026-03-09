namespace Blackset.Duel.Targets
{
    /// <summary>
    /// Цель применения расходника
    /// </summary>
    public enum ApplyTarget
    {
        None, // Без цели (расходник не применялся)
        Self, // На себя
        Opponent, // На соперника
        Both // На всех
    }
}