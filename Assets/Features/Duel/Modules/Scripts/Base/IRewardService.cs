using Blackset.Duel.Requests;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Сервис поулчения игроком награды за дуэль
    /// </summary>
    public interface IRewardService : IDuelModuleInterface
    {
        /// <summary>
        /// Создание списка наград
        /// </summary>
        /// <param name="request">Запрос награждения</param>
        /// <returns>Список наград</returns>
        public DuelRewards BuildReward(RewardRequest request);
        /// <summary>
        /// Применить (получить) награду
        /// </summary>
        /// <param name="result">Список наград</param>
        public void ApplyResult(DuelRewards result);
    }
}