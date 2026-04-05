namespace Blackset.Duel.Rules
{
    /// <summary>
    /// 
    /// </summary>
    public enum FightLossPolicy
    {
        /// <summary>
        /// Бой до выбивания ЦЗ одним из участников ИЛИ бой до конца (проигрывает кто набрал меньше очков)
        /// </summary>
        LessOrBust,
        /// <summary>
        /// Бой до конца (проигрывает кто набрал меньше очков)
        /// </summary>
        Less, 
        /// <summary>
        /// Бой до конца (проигрывает кто был дальше от ЦЗ)
        /// </summary>
        Furthest 
    }
}