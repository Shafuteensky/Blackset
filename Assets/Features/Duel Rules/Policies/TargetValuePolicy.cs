namespace Blackset.Duel.Rules
{
    /// <summary>
    /// Политика создания целевого значения
    /// </summary>
    public enum TargetValuePolicy
    {
        RandomSet = 0, // Бросок набора дайсов по правилам
        RandomSetByDicesInSet = 1, // Бросок стандартного набора дайсов (в количестве дайсов в сборке по правилам)
        Blackjack = 2, // Фиксированное: 21
    }
}