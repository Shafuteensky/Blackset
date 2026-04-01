namespace Blackset.Duel.Rules
{
    /// <summary>
    /// Политика составления сборок дайсов участников
    /// </summary>
    public enum DiceSetPolicy
    {
        OneInType = 0, // Один случайный дайс на каждый тип
        Chaos = 1, // Все дайсы случайны
        AllD20 = 2 // Все дайсы трансформируюстя в D20
    }
}