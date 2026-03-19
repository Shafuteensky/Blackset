using Blackset.Duel.Requests;
using Blackset.Duel.TargetValue;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Генератор целевого значения
    /// </summary>
    public interface ITargetValueGenerator : IDuelModuleInterface
    {
        /// <summary>
        /// Сгенерировать целевое значение
        /// </summary>
        /// <param name="request">Запрос на генерацию</param>
        /// <returns>Данные о целевом значении</returns>
        public TargetValueContext Generate(TargetValueRequest request);
    }
}