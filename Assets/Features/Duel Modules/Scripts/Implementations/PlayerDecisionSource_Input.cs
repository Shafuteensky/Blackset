using System.Threading;
using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Cysharp.Threading.Tasks;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Получение намерений игрока через ввод с UI.
    /// Реализует <see cref="IDuelInputHandler"/> — UI обращается напрямую к этому классу.
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlayerDecisionSource_Input),
        menuName = "Blackset/Duel/Modules/" + nameof(PlayerDecisionSource_Input))]
    public class PlayerDecisionSource_Input : BaseDuelModule, IPlayerDecisionSource, IDuelInputHandler
    {
        private DuelContext duelContext;

        private string expectedParticipantId;
        private bool awaitingInput;
        
        private UniTaskCompletionSource<SelectionState> inputCompletionSource;

        #region IPlayerDecisionSource (Запрос на ввод)

        /// <summary>
        /// Получить ввод выбора от игрока
        /// </summary>
        public UniTask<SelectionState> GetSelection(DuelContext context, CancellationToken cancellationToken)
        {
            ServiceGuard.NotNull(context, nameof(context));

            duelContext = context;

            inputCompletionSource = new UniTaskCompletionSource<SelectionState>();
            expectedParticipantId = context.PlayerId;
            awaitingInput = true;

            cancellationToken.Register(CancelInput);

            return inputCompletionSource.Task;
        }

        #endregion

        #region IDuelInputHandler (Ввод)

        /// <summary>
        /// Вызывается UI при выборе дайса участником
        /// </summary>
        public void OnDiceSelected(string participantId, SelectionState selection)
        {
            if (!CanAccept(participantId)) return;

            CompleteInput(selection);
        }

        /// <summary>
        /// Вызывается UI при выборе расходника участником
        /// </summary>
        public void OnConsumableSelected(string participantId, SelectionState selection)
        {
            if (!CanAccept(participantId)) return;

            CompleteInput(selection);
        }

        /// <summary>
        /// Вызывается UI при запросе паса
        /// </summary>
        public void OnPassRequested()
        {
            if (!awaitingInput) return;

            duelContext.Participants[duelContext.PlayerId].FightState.TurnState.MarkPassed();
            CompleteInput(new SelectionState());
        }

        #endregion

        #region Завершение ввода

        private void CompleteInput(SelectionState result)
        {
            awaitingInput = false;
            expectedParticipantId = string.Empty;
            inputCompletionSource.TrySetResult(result);
        }

        private void CancelInput()
        {
            awaitingInput = false;
            expectedParticipantId = string.Empty;
            inputCompletionSource.TrySetCanceled();
        }

        #endregion

        /// <summary>
        /// Проверяет, может ли текущий ввод быть принят от данного участника
        /// </summary>
        private bool CanAccept(string participantId) =>
            awaitingInput && participantId == expectedParticipantId;
    }
}