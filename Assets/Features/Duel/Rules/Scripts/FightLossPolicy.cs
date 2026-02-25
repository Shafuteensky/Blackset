namespace Blackset.Duel.Rules
{
    /// <summary>
    /// 
    /// </summary>
    public enum FightLossPolicy
    {
        LessOrBust, // Бой до выбивания ЦЗ одним из участников ИЛИ бой до конца (проигрывает кто набрал меньше очков)
        Less // Бой до конца (проигрывает кто набрал меньше очков)
    }
}