using System.Threading;
using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.TurnIntents;
using Cysharp.Threading.Tasks;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Получение намерений игрока через ввод с UI с ожиданием
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlayerDecisionSource_Input),
        menuName = "Blackset/Duel/Modules/" + nameof(PlayerDecisionSource_Input))]
    public class PlayerDecisionSource_Input : BaseDuelModule, IPlayerDecisionSource
    {
        private DuelInputPresenter presenter;

        public void Initialize(DuelInputPresenter newPresenter)
        {
            presenter = newPresenter;
            isInitialized = true;
        }
        
        public UniTask<string> GetDeclaration(DuelContext context, CancellationToken ct)
        {
            ServiceGuard.NotNull(context, nameof(context));
            ServiceGuard.NotNull(presenter, nameof(presenter));

            var tcs = new UniTaskCompletionSource<string>();

            var reg = ct.Register(() =>
            {
                presenter.Hide();
                tcs.TrySetCanceled();
            });

            presenter.ShowDeclarationUI(context, intent =>
            {
                reg.Dispose();
                presenter.Hide();
                tcs.TrySetResult(intent);
            });
            
            return tcs.Task;
        }

        // TODO Использование в FSM: var intent = await decisionSource.GetTurnIntent(context, ct);
        public UniTask<TurnIntent> GetTurnIntent(DuelContext context, CancellationToken ct)
        {
            ServiceGuard.NotNull(context, nameof(context));
            ServiceGuard.NotNull(presenter, nameof(presenter));

            var tcs = new UniTaskCompletionSource<TurnIntent>();

            var reg = ct.Register(() =>
            {
                presenter.Hide();
                tcs.TrySetCanceled();
            });

            presenter.ShowTurnIntentUI(context, intent =>
            {
                reg.Dispose();
                presenter.Hide();
                tcs.TrySetResult(intent);
            });

            return tcs.Task;
        }
    }
}