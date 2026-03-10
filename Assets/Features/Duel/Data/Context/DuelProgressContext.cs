using Extensions.Reactive;
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
            
        public ReactiveProperty<int> FightNumber { get; private set; }
        /// <summary>
        /// Номер текущего броска в этой битве
        /// </summary>
        public ReactiveProperty<int> ThrowNumber { get; private set; }
        /// <summary>
        /// Состояние дуэли
        /// </summary>
        public ReactiveProperty<bool> IsDuelFinished { get; private set; }
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
            FightNumber.Value++;
            ThrowNumber.Value = 0;
        }

        /// <summary>
        /// Начало нового хода (броска)
        /// </summary>
        public void OnNewThrow()
        {
            ThrowNumber.Value++;
        }

        /// <summary>
        /// Завершение дуэли
        /// </summary>
        public void OnDuelFinished(DuelEndResult duelResult)
        {
            DuelResult = duelResult;
            IsDuelFinished.Value = true;
        }
        
        #endregion
    }
}