using Features.Duel.Data.FightEnd;
using Features.Duel.Requests;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Резолвер окончания боя
    /// </summary>
    public interface IFightEndResolver : IDuelModuleInterface
    {
        /// <summary>
        /// Определение победителя в бою
        /// </summary>
        /// <param name="request">Запрос на завершение боя</param>
        /// <returns>Результаты боя</returns>
        public FightEndResult Evaluate(FightEndRequest request);
    }
}