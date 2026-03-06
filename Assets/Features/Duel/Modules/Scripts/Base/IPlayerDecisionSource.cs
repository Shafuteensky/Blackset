using System.Threading;
using Blackset.Duel.Context;
using Blackset.Duel.TurnIntents;
using Cysharp.Threading.Tasks;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Источник решений игрока
    /// </summary>
    public interface IPlayerDecisionSource : IDuelModuleInterface
    {
        /// <summary>
        /// Запрос на объявление дайса на бросок
        /// </summary>
        /// <remarks>
        /// На этом этапе происходит ожидание ввода от игрока в UI
        /// </remarks>
        /// <param name="context">Дунные дуэли</param>
        /// <param name="ct">Токен отмены ожидания</param>
        /// <returns>Задача с ожиданием результата выбора</returns>
        public UniTask<string> GetDeclaration(DuelContext context, CancellationToken ct);
        /// <summary>
        /// Запрос на создание данных о намерении игрока
        /// </summary>
        /// <remarks>
        /// На этом этапе происходит ожидание ввода от игрока в UI
        /// </remarks>
        /// <param name="context">Дунные дуэли</param>
        /// <param name="ct">Токен отмены ожидания</param>
        /// <returns>Задача с ожиданием результата выбора</returns>
        public UniTask<TurnIntent> GetTurnIntent(DuelContext context, CancellationToken ct);
       
    }
}