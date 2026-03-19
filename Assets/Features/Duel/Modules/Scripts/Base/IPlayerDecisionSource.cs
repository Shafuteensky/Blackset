using System.Threading;
using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Cysharp.Threading.Tasks;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Источник решений игрока
    /// </summary>
    public interface IPlayerDecisionSource : IDuelModuleInterface
    {
        /// <summary>
        /// Запрос на ввод выбора игрока
        /// </summary>
        /// <remarks>
        /// На этом этапе происходит ожидание ввода от игрока в UI
        /// </remarks>
        /// <param name="context">Данные дуэли</param>
        /// <param name="cancellationToken">Токен отмены ожидания</param>
        /// <returns>Задача с ожиданием результата выбора</returns>
        public UniTask<SelectionState> GetSelection(DuelContext context, CancellationToken cancellationToken);
    }
}