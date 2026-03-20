namespace Blackset.Duel.Rules
{
    /// <summary>
    /// Политика составления сборок расходников участников
    /// </summary>
    public enum ConsumableSetPolicy
    {
        /// <summary>
        /// Все расходники случайные, с лимитом по количеству
        /// </summary>
        RandomLimited,
        /// <summary>
        /// Без расходников
        /// </summary>
        None
    }
}