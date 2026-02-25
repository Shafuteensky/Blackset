namespace Blackset.Duel.Rules
{
    /// <summary>
    /// Политика победы в битве в составе дуэли
    /// </summary>
    public enum FightWinPolicy
    {
        ExactOrClosest, // Бой до попадания в ЦЗ одним из участников ИЛИ бой до конца (побеждает кто набрал больше очков)
        ClosestOnly // Бой до конца (побеждает кто набрал больше очков)
    }
}