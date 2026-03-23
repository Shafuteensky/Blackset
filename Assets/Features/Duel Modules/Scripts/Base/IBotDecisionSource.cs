using Blackset.DecisionInput;
using Blackset.Duel.Context;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Источник построения намерений бота
    /// </summary>
    public interface IBotDecisionSource : IDuelModuleInterface
    {
        /// <summary>
        /// Создать объявление дайса на бросок
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        /// <returns>Идентификатор объявленного дайса</returns>
        public SelectionState BuildDeclaration(DuelContext context);
        /// <summary>
        /// Создать выбор дайса на бросок
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        /// <returns>Состояние намерений бота на ход</returns>
        public SelectionState BuildDiceSelection(DuelContext context);
        /// <summary>
        /// Создать выбор расходника на бросок
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        /// <returns>Состояние намерений бота на ход</returns>
        public SelectionState BuildConsumableSelection(DuelContext context);
    }
}