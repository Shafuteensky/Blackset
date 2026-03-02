using System;

namespace Features.Duel.Data.FightEnd
{
    /// <summary>
    /// Результат боя
    /// </summary>
    public struct FightEndResult
    {
        /// <summary>
        /// Завершен ли бой 
        /// </summary>
        /// <returns>
        /// true если выполнены все действия по правилам, false если бой был прерван)
        /// </returns>
        public bool IsFightEnded { get; }
        /// <summary>
        /// Победитель в бою
        /// </summary>
        public FightWinner Winner { get; }
        /// <summary>
        /// Идентификатор победившего участника
        /// </summary>
        public string WinnerId { get; }
        /// <summary>
        /// Причина завершения боя
        /// </summary>
        public FightEndReason Reason { get; }

        /// <summary>
        /// Данные результата боя
        /// </summary>
        /// <param name="isFightEnded">Завершен ли бой</param>
        /// <param name="winner">Победитель в бою</param>
        /// <param name="winnerId">Идентификатор победившего участника</param>
        /// <param name="reason">Причина завершения боя</param>
        public FightEndResult(bool isFightEnded, FightWinner winner, string winnerId, FightEndReason reason)
        {
            IsFightEnded = isFightEnded;
            Winner = winner;
            WinnerId = winnerId;
            Reason = reason;
        }
    }
}