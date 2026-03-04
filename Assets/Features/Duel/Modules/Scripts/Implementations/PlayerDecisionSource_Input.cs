using System;
using System.Threading;
using Blacklset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.TurnIntents;
using Cysharp.Threading.Tasks;
using Extensions.Log;
using UnityEngine;

namespace Blacklset.Duel.Modules
{
    /// <summary>
    /// Получение намерений игрока через ввод с UI с ожиданием
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlayerDecisionSource_Input),
        menuName = "Blackset/Duel/Modules/" + nameof(PlayerDecisionSource_Input))]
    public class PlayerDecisionSource_Input : BaseDuelModule, IPlayerDecisionSource
    {
        public UniTask<string> GetDeclaration(DuelContext context, CancellationToken ct)
        {
            ServiceGuard.NotNull(context, nameof(context));
            DuelInputPresenter presenter = ResolvePresenter(context);

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
            DuelInputPresenter presenter = ResolvePresenter(context);

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

        private DuelInputPresenter ResolvePresenter(DuelContext context)
        {
            if (context.InputPresenter == null)
            {
                throw new NullReferenceException(
                    $"{nameof(DuelContext)}.{nameof(DuelContext.InputPresenter)} is null. " +
                    $"Assign presenter before requesting input.");
            }

            return context.InputPresenter;
        }
    }
}