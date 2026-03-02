using Blackset.Duel.Context;
using Blackset.Duel.TurnIntents;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Источник построения намерений бота
    /// </summary>
    public interface IBotDecisionSource
    {
        /// <summary>
        /// Создать намерение бота
        /// </summary>
        /// <param name="context">Даннеы дуэли</param>
        /// <returns>Намерение бота</returns>
        public TurnIntent BuildTurnIntent(DuelContext context);
    }
}