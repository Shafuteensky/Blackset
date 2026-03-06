using Blackset.Duel.Requests;
using Blackset.Duel.Sets;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Генератор сборок участника дуэли
    /// </summary>
    public interface IParticipantSetGenerator : IDuelModuleInterface
    {
        /// <summary>
        /// Генерация сборок предметов учатсника дуэли
        /// </summary>
        /// <param name="request">Запрос на генерацию сборок</param>
        /// <returns>Сборки участника дуэли</returns>
        public DuelSetsContext GenerateSets(SetGenerationRequest request);
    }
}