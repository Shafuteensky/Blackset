using Blackset.Duel.Context;
using Features.Duel.Data.FightEnd;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Резолвер счетов дуэли участников дуэли
    /// </summary>
    public interface IDuelScoreResolver : IDuelModuleInterface
    {
        /// <summary>
        /// Зачет очков за победу в дуэли
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        /// <param name="fightEndResult">Данные об окончании битвы</param>
        public void ResolveDuelWin(DuelContext context, FightEndResult fightEndResult);
        
        /// <summary>
        /// Зачет очков за неиспользованные предметы
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        public void ResolveUnusedItems(DuelContext context);

        /// <summary>
        /// Зачет очков за честность
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        public void ResolveHonesty(DuelContext context, string participantId, string declared, string chosen);

        /// <summary>
        /// Зачет очков за криты дайсов
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        public void ResolveCrit(DuelContext context, string participantId, bool isCrit);
    }
}