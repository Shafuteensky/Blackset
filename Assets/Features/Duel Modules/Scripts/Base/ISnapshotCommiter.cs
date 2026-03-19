using Blackset.Duel.Context;
using Blackset.Duel.Snapshots;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Перенос финально рассчитанных данных хода из снапшота в "истину" дуэли
    /// </summary>
    public interface ISnapshotCommiter : IDuelModuleInterface
    {
        /// <summary>
        /// Коммит снапшота
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        /// <param name="resolvedSnapshot">Данные зарезолвенного снапшота</param>
        public void Commit(TurnSnapshot resolvedSnapshot, DuelContext context);
    }
}