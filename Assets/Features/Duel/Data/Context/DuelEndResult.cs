using Features.Duel.Data.FightEnd;

namespace Features.Duel.Context
{
    /// <summary>
    /// Результат дуэли
    /// </summary>
    public struct DuelEndResult
    {
        /// <summary>
        /// Завершена ли дуэль 
        /// </summary>
        /// <returns>
        /// true если выполнены все действия по правилам, false если бой дуэль продолжается)
        /// </returns>
        public bool IsDuelEnded { get; set; }
        /// <summary>
        /// Победитель в бою
        /// </summary>
        public FightWinner Winner { get; set; }
        /// <summary>
        /// Идентификатор победившего участника
        /// </summary>
        public string WinnerId { get; set; }

        /// <summary>
        /// Данные результата боя
        /// </summary>
        /// <param name="isFightEnded">Завершена ли дуэль</param>
        /// <param name="winner">Победитель в дуэли</param>
        /// <param name="winnerId">Идентификатор победившего участника</param>
        public DuelEndResult(bool isFightEnded, string winnerId, FightWinner winner)
        {
            IsDuelEnded = isFightEnded;
            Winner = winner;
            WinnerId = winnerId;
        }
    }
}