using System.Threading;
using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Cysharp.Threading.Tasks;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Получение намерений игрока через ввод с UI
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlayerDecisionSource_Input),
        menuName = "Blackset/Duel/Modules/" + nameof(PlayerDecisionSource_Input))]
    public class PlayerDecisionSource_Input : BaseDuelModule, IPlayerDecisionSource
    {
        private DuelContext duelContext;
        private DuelInputPresenter presenter;

        private UniTaskCompletionSource<SelectionState> inputCompletionSource;

        private bool awaitingInput;

        private SelectionState selection = new();

        /// <summary>
        /// Инициализация зависимостей
        /// </summary>
        public void Initialize(DuelInputPresenter newPresenter)
        {
            presenter = newPresenter;
            isInitialized = true;

            presenter.SetItemCallbacks(
                HandleInput,
                HandleInput,
                HandlePassRequested);
        }

        #region Ввод
        
        /// <summary>
        /// Получить ввод выбора от игрока
        /// </summary>
        public UniTask<SelectionState> GetSelection(DuelContext context, CancellationToken cancellationToken)
        {
            ServiceGuard.NotNull(context, nameof(context));
            ServiceGuard.NotNull(presenter, nameof(presenter));
            
            duelContext = context;

            selection = new SelectionState();
            inputCompletionSource = new UniTaskCompletionSource<SelectionState>();
            awaitingInput = true;

            cancellationToken.Register(CancelInput);

            presenter.BeginSelectionInput(context.PlayerId);

            return inputCompletionSource.Task;
        }
        
        #endregion

        #region Обработка ввода
        
        private void HandleInput(SelectionState inputSelection)
        {
            if (!awaitingInput) return;
            
            selection = inputSelection;
            CompleteInput(selection);
        }

        private void HandlePassRequested()
        {
            duelContext.Participants[duelContext.PlayerId].FightState.TurnState.MarkPassed();
            
            if (awaitingInput)
            {
                SelectionState emptySelection = new SelectionState();
                CompleteInput(emptySelection);
            }
        }
        
        #endregion

        #region Завершение ввода
        
        private void CompleteInput(SelectionState inputSelection)
        {
            awaitingInput = false;
            presenter.EndInput();
            inputCompletionSource.TrySetResult(selection);
        }
        
        private void CancelInput()
        {
            awaitingInput = false;
            presenter.EndInput();
            inputCompletionSource.TrySetCanceled();
        }
        
        #endregion
    }
}