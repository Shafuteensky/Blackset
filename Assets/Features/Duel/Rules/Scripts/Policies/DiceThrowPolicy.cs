namespace Blackset.Duel.Rules
{
    /// <summary>
    /// Политика броска дайса в течении одной битвы
    /// </summary>
    public enum DiceThrowPolicy
    {
        Once, // Каждый дайс можно использовать лишь раз
        NotLimited // Каждый дайс можно кинуть не ограниченное количество раз
    }
}