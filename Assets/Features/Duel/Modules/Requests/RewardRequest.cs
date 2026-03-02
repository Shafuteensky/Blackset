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
        public string ContractId { get; }

        /// <summary>
        /// Запрос на выдачу награды игроку за дуэль
        /// </summary>
        /// <param name="isWin">Победил ли игрок в дуэли</param>
        /// <param name="contractId">Идентификатор завершённого контракта</param>
        public RewardRequest(bool isWin, string contractId)
        {
            IsWin = isWin;
            ContractId = contractId;
        }
    }
}