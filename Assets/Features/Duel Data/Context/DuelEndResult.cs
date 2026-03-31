using Extensions.Reactive;
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
        public ReactiveProperty<bool> IsDuelEnded { get; set; }
        /// <summary>
        /// Победитель в бою
        /// </summary>
        public ReactiveProperty<FightWinner> Winner { get; set; }
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
            IsDuelEnded = new ReactiveProperty<bool>(isFightEnded)
            {
                Value = isFightEnded
            };
            Winner = new ReactiveProperty<FightWinner>(winner)
            {
                Value = winner
            };
            WinnerId = winnerId;
        }

        /// <summary>
        /// Чистый результат
        /// </summary>
        public static DuelEndResult Clear(bool fightEnded = false) => new(fightEnded, string.Empty, FightWinner.None);
    }
}