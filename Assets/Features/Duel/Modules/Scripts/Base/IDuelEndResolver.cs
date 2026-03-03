using Blackset.Duel.Context;
using Features.Duel.Context;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Резолвер окончания дуэли
    /// </summary>
    public interface IDuelEndResolver : IDuelModuleInterface
    {
        /// <summary>
        /// Определение победителя в дуэли
        /// </summary>
        /// <param name="DuelContext">Данные дуэли</param>
        /// <returns>Результаты боя</returns>
        public DuelEndResult Evaluate(DuelContext context);
    }
}