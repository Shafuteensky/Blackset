using Blackset.Duel.Context;
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
        /// <param name="DuelContext">Данные дуэли</param>
        /// <returns>Результаты боя</returns>
        public FightEndResult Evaluate(DuelContext context);
    }
}