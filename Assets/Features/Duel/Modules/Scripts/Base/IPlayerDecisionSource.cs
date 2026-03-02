using System.Threading;
using Blackset.Duel.Context;
using Blackset.Duel.TurnIntents;
using Cysharp.Threading.Tasks;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Источник решений игрока
    /// </summary>
    public interface IPlayerDecisionSource : IDuelModuleInterface
    {
        /// <summary>
        /// Запрос на создание данных о намерении игрока
        /// </summary>
        /// <remarks>
        /// На этом этапе происходит ожидание ввода от игрока в UI
        /// </remarks>
        /// <param name="context">Дунные дуэли</param>
        public UniTask<TurnIntent> GetTurnIntent(DuelContext context, CancellationToken ct);
       
    }
}