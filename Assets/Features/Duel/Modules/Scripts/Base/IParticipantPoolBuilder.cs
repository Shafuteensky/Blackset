using Blackset.Duel.Pools;
using Blackset.Duel.Requests;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Билдер пулов участников
    /// </summary>
    public interface IParticipantPoolBuilder : IDuelModuleInterface
    {
        /// <summary>
        /// Заполнение данных пулов предметов участников
        /// </summary>
        /// <param name="request">Запрос на построение пулов</param>
        /// <returns>Пулы кубов и расходников участника</returns>
        public DuelPoolsContext BuildPools(PoolBuildRequest request);
    }
}