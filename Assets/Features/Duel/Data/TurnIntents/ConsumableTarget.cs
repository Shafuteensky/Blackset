namespace Blackset.Duel.TurnIntents
{
    /// <summary>
    /// Цель применения расходника
    /// </summary>
    public enum ConsumableTarget
    {
        None, // Без цели (расходник не применялся)
        Self, // На себя
        Opponent, // На соперника
        Both // На всех
    }
}