using Features.Duel.Context;

namespace Blackset.Duel.Context
{
    /// <summary>
    /// Прогресс активной дуэли
    /// </summary>
    public struct DuelProgressContext
    {
        /// <summary>
        /// Номер активной битвы
        /// </summary>
        public int FightNumber { get; private set; }
        /// <summary>
        /// Номер текущего броска в этой битве
        /// </summary>
        public int ThrowNumber { get; private set; }
        /// <summary>
        /// Состояние дуэли
        /// </summary>
        public bool IsDuelFinished { get; private set; }
        /// <summary>
        /// Результат окончания дуэли
        /// </summary>
        public DuelEndResult DuelResult { get; private set; }

        #region Обновление прогресса дуэли
        
        /// <summary>
        /// Начало новой битвы
        /// </summary>
        public void OnNewFight()
        {
            FightNumber++;
            ThrowNumber = 0;
        }

        /// <summary>
        /// Начало нового хода (броска)
        /// </summary>
        public void OnNewThrow()
        {
            ThrowNumber++;
        }

        /// <summary>
        /// Завершение дуэли
        /// </summary>
        public void OnDuelFinished(DuelEndResult duelResult)
        {
            DuelResult = duelResult;
            IsDuelFinished = true;
        }
        
        #endregion
    }
}