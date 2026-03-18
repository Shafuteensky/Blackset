using Blackset.DuelContracts;

namespace Blackset.Duel.Requests
{
    /// <summary>
    /// Запрос на выдачу награды игроку за дуэль
    /// </summary>
    public struct RewardRequest
    {
        /// <summary>
        /// Победил ли игрок в буэли
        /// </summary>
        public bool IsWin { get; }
        /// <summary>
        /// Завершенный контракт
        /// </summary>
        public DuelContract Contract { get; }
        /// <summary>
        /// Счет дуэли игрока
        /// </summary>
        public int PlayerDuelScore { get; }

        /// <summary>
        /// Запрос на выдачу награды игроку за дуэль
        /// </summary>
        /// <param name="isWin">Победил ли игрок в дуэли</param>
        /// <param name="contractId">Завершенный контракт</param>
        public RewardRequest(bool isWin, DuelContract contract, int playerDuelScore)
        {
            IsWin = isWin;
            Contract = contract;
            PlayerDuelScore = playerDuelScore;
        }
    }
}