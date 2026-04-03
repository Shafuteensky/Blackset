namespace Blackset.OpponentsRestrictions
{
    /// <summary>
    /// Доступность соперника
    /// </summary>
    [System.Flags]
    public enum OpponentAvailability
    {
        None = 0,
        All = ~0,
        Duel = 1 << 0,
        League = 1 << 1
    }
}