using Blackset.Duel.Context;

namespace Blackset.Duel.TurnSnapshot
{
    /// <summary>
    /// Резолвер данных хода (снапшота)
    /// </summary>
    public class SnapshotCommiter
    {
        /// <summary>
        /// Коммит снапшота
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        /// <param name="snapshot">Данные зарезолвенного снапшота</param>
        public void Commit(DuelContext context, ResolvedSnapshot snapshot)
        {
            // TODO применение данных игры
        }
    }
}