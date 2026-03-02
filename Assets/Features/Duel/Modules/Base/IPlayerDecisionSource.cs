using Blackset.Duel.Context;
using Blackset.Duel.TurnIntents;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Источник решений игрока
    /// </summary>
    public interface IPlayerDecisionSource
    {
        /// <summary>
        /// Запрос на создание данных о намерении игрока
        /// </summary>
        /// <remarks>
        /// На этом этапе происходит ожидание ввода от игрока в UI
        /// </remarks>
        /// <param name="context">Дунные дуэли</param>
        public void RequestTurnIntent(DuelContext context);
        /// <summary>
        /// Получение собранных данных о намерении игрока
        /// </summary>
        /// <remarks>
        /// На этом этапе происходит отклик после того как игрок завершил ввод
        /// </remarks>
        /// <returns>Намерение игрока на текущий ход</returns>
        public TurnIntent ConsumeTurnIntent();
    }
}