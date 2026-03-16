namespace Blackset.ItemsRestrictions
{
    /// <summary>
    /// Доступность предмета игроку 
    /// </summary>
    [System.Flags]
    public enum ItemAvailability
    {
        None = 0,
        All = ~0,
        Reward = 1 << 0,
        Purchase = 1 << 1
    }
}