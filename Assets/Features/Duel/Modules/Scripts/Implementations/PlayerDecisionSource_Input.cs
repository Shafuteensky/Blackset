using System.Threading;
using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Duel.Targets;
using Cysharp.Threading.Tasks;
using Extensions.Log;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Получение намерений игрока через ввод с UI.
    /// Владеет игровым состоянием текущего выбора.
    /// </summary>
    [UnityEngine.CreateAssetMenu(
        fileName = nameof(PlayerDecisionSource_Input),
        menuName = "Blackset/Duel/Modules/" + nameof(PlayerDecisionSource_Input))]
    public class PlayerDecisionSource_Input : BaseDuelModule, IPlayerDecisionSource
    {
        private DuelInputPresenter presenter;

        private string selectedDiceId;
        private TurnParticipantState currentIntent;

        private UniTaskCompletionSource<string> declarationTcs;
        private UniTaskCompletionSource<TurnParticipantState> intentTcs;

        /// <summary>
        /// Инициализация зависимостей
        /// </summary>
        public void Initialize(DuelInputPresenter newPresenter)
        {
            presenter = newPresenter;
            isInitialized = true;

            presenter.SetItemCallbacks(HandleDiceSelected, HandleConsumableSelected);
        }

        /// <summary>
        /// Получить объявление дайса от игрока
        /// </summary>
        public UniTask<string> GetDeclaration(DuelContext context, CancellationToken ct)
        {
            ServiceGuard.NotNull(context, nameof(context));
            ServiceGuard.NotNull(presenter, nameof(presenter));

            selectedDiceId = string.Empty;
            declarationTcs = new UniTaskCompletionSource<string>();

            ct.Register(CancelDeclaration);

            presenter.ShowDeclarationUI(onConfirm: ConfirmDeclaration);

            return declarationTcs.Task;
        }

        /// <summary>
        /// Получить намерения игрока на ход
        /// </summary>
        public UniTask<TurnParticipantState> GetIntentState(DuelContext context, CancellationToken ct)
        {
            ServiceGuard.NotNull(context, nameof(context));
            ServiceGuard.NotNull(presenter, nameof(presenter));

            currentIntent = new TurnParticipantState();
            currentIntent.ResetForNewTurn();
            intentTcs = new UniTaskCompletionSource<TurnParticipantState>();

            ct.Register(CancelIntent);

            presenter.ShowTurnIntentUI(onConfirm: ConfirmIntent, onPass: PassIntent);

            return intentTcs.Task;
        }

        #region Обработка выбора предметов

        private void HandleDiceSelected(string diceId)
        {
            selectedDiceId = diceId;
            currentIntent?.ChoseDice(diceId);
        }

        private void HandleConsumableSelected(string consumableId, ApplyTarget target)
        {
            currentIntent?.ChoseConsumable(consumableId, target);
        }

        #endregion

        #region Обработка подтверждений и отмен

        private void ConfirmDeclaration()
        {
            presenter.Hide();
            declarationTcs.TrySetResult(selectedDiceId);
        }

        private void CancelDeclaration()
        {
            presenter.Hide();
            declarationTcs.TrySetCanceled();
        }

        private void ConfirmIntent()
        {
            presenter.Hide();
            intentTcs.TrySetResult(currentIntent);
        }

        private void PassIntent()
        {
            TurnParticipantState passIntent = new TurnParticipantState();
            passIntent.ResetForNewTurn();
            passIntent.MarkPassed();

            presenter.Hide();
            intentTcs.TrySetResult(passIntent);
        }

        private void CancelIntent()
        {
            presenter.Hide();
            intentTcs.TrySetCanceled();
        }

        #endregion
    }
}