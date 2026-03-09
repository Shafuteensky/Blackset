using Blackset.Duel.Context;
using Blackset.Duel.Participants;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Источник построения намерений бота
    /// </summary>
    public interface IBotDecisionSource : IDuelModuleInterface
    {
        /// <summary>
        /// Создать выбор объявления дайса на бросок
        /// </summary>
        /// <param name="context">Дунные дуэли</param>
        /// <returns>Идентификатор объявленного дайса</returns>
        public string BuildDeclaration(DuelContext context);
        /// <summary>
        /// Создать намерение бота
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        /// <returns>Состояние намерений бота на ход</returns>
        public TurnParticipantState BuildIntentState(DuelContext context);
    }
}