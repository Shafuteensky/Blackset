namespace Blackset.Duel.Rules
{
    /// <summary>
    /// Политика создания целевого значения
    /// </summary>
    public enum TargetValuePolicy
    {
        RandomSet, // Бросок стандартного набора дайсов
        RandomSetByDicesInSet, // Бросок стандартного набора дайсов (в количестве дайсов в сборке по правилам)
        Blackjack // Фиксированное: 21
    }
}