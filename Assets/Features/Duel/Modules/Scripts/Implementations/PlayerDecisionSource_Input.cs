using System.Threading;
using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Duel.Targets;
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
        private DuelInputPresenter presenter;

        private string selectedDiceId;
        private TurnParticipantState currentIntent;

        private UniTaskCompletionSource<string> declarationTcs;
        private UniTaskCompletionSource<TurnParticipantState> intentTcs;

        private bool declarationAwaiting;
        private bool intentAwaiting;

        private DuelContext duelContext;

        /// <summary>
        /// Инициализация зависимостей
        /// </summary>
        public void Initialize(DuelInputPresenter newPresenter)
        {
            presenter = newPresenter;
            isInitialized = true;

            presenter.SetItemCallbacks(
                HandleDiceSelected,
                HandleConsumableSelected,
                HandlePassRequested);
        }

        /// <summary>
        /// Получить объявление дайса от игрока
        /// </summary>
        public UniTask<string> GetDeclaration(DuelContext context, CancellationToken ct)
        {
            ServiceGuard.NotNull(context, nameof(context));
            ServiceGuard.NotNull(presenter, nameof(presenter));
            
            duelContext = context;
                
            selectedDiceId = string.Empty;
            declarationAwaiting = true;
            declarationTcs = new UniTaskCompletionSource<string>();

            ct.Register(CancelDeclaration);

            presenter.BeginDeclaration(context.PlayerId);

            return declarationTcs.Task;
        }

        /// <summary>
        /// Получить намерения игрока на ход
        /// </summary>
        public UniTask<TurnParticipantState> GetIntentState(DuelContext context, CancellationToken ct)
        {
            ServiceGuard.NotNull(context, nameof(context));
            ServiceGuard.NotNull(presenter, nameof(presenter));

            duelContext = context;
            
            currentIntent = new TurnParticipantState();
            currentIntent.ResetForNewTurn();

            intentAwaiting = true;
            intentTcs = new UniTaskCompletionSource<TurnParticipantState>();

            ct.Register(CancelIntent);

            presenter.BeginIntentSelection(context.PlayerId);

            return intentTcs.Task;
        }

        #region Обработка
        
        private void HandleDiceSelected(string diceId)
        {
            if (declarationAwaiting)
            {
                selectedDiceId = diceId;
                CompleteDeclaration();
                return;
            }

            if (!intentAwaiting) return;

            currentIntent.ChoseDice(diceId);
            CompleteIntent();
        }

        private void HandleConsumableSelected(string consumableId, ApplyTarget target)
        {
            if (!intentAwaiting) return;
            currentIntent.ChoseConsumable(consumableId, target);
        }

        private void HandlePassRequested()
        {
            duelContext.Participants[duelContext.PlayerId].FightState.TurnState.MarkPassed();
            
            if (declarationAwaiting)
            {
                CompleteDeclarationPass();
                return;
            }

            if (intentAwaiting) CompleteIntentPass();
        }

        private void CompleteDeclaration()
        {
            declarationAwaiting = false;
            presenter.EndInput();
            declarationTcs.TrySetResult(selectedDiceId);
        }

        private void CompleteDeclarationPass()
        {
            declarationAwaiting = false;
            presenter.EndInput();
            declarationTcs.TrySetResult(string.Empty);
        }

        private void CancelDeclaration()
        {
            declarationAwaiting = false;
            presenter.EndInput();
            declarationTcs.TrySetCanceled();
        }

        private void CompleteIntent()
        {
            intentAwaiting = false;
            presenter.EndInput();
            intentTcs.TrySetResult(currentIntent);
        }

        private void CompleteIntentPass()
        {
            TurnParticipantState passIntent = new TurnParticipantState();
            passIntent.ResetForNewTurn();
            passIntent.MarkPassed();

            intentAwaiting = false;
            presenter.EndInput();
            intentTcs.TrySetResult(passIntent);
        }

        private void CancelIntent()
        {
            intentAwaiting = false;
            presenter.EndInput();
            intentTcs.TrySetCanceled();
        }
        
        #endregion
    }
}