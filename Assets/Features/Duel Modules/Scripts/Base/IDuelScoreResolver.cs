using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Duel.Snapshots;
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
        public void ResolveDuelWin(DuelContext context, FightEndResult fightEndResult, TurnSnapshot snapshot);
        
        /// <summary>
        /// Зачет очков за неиспользованные предметы
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        public void ResolveUnusedItems(DuelContext context, TurnSnapshot snapshot);

        /// <summary>
        /// Зачет очков за честность
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        public void ResolveHonesty(string participantId, string declared, string chosen, TurnSnapshot snapshot);

        /// <summary>
        /// Зачет очков за криты дайсов
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        public void ResolveCrit(string participantId, bool isCrit, TurnSnapshot snapshot);
    }
}