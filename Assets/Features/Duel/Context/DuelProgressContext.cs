namespace Blackset.Duel.Context
{
    /// <summary>
    /// Прогресс активной дуэли
    /// </summary>
    public struct DuelProgressContext
    {
        /// <summary>
        /// Индекс активной битвы
        /// </summary>
        public int FightIndex;
        /// <summary>
        /// Индекс активного броска в этой битве
        /// </summary>
        public int ThrowIndex;
        /// <summary>
        /// Состояние дуэли
        /// </summary>
        public bool IsDuelFinished;
    }
}