using Blackset.Duel.Context;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Пайплайн броска дайса для получения численного результата
    /// </summary>
    public interface IDiceRollPipeline : IDuelModuleInterface
    {
        /// <summary>
        /// Бросить выбранный дайс
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        /// <param name="participantId">Идентификатор участника</param>
        /// <returns>Результат броска</returns>
        public int RollChosenDice(DuelContext context, string participantId);
        /// <summary>
        /// Бросить определенный дайс
        /// </summary>
        /// <param name="context">Данные дуэли</param>
        /// <param name="participantId">Идентификатор участника</param>
        /// <param name="diceId">Идентификатор дайса из сборки участника</param>
        /// <returns>Результат броска</returns>
        public int RollDice(DuelContext context, string participantId, string diceId);
    }
}