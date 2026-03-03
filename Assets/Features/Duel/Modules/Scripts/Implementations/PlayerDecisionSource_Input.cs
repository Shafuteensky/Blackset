using System.Threading;
using Blackset.Duel.Context;
using Blackset.Duel.TurnIntents;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Получение намерений игрока через ввод с UI с ожиданием
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlayerDecisionSource_Input),
        menuName = "Blackset/Duel/Modules/" + nameof(PlayerDecisionSource_Input))]
    public class PlayerDecisionSource_Input : BaseDuelModule, IPlayerDecisionSource
    {
        public UniTask<TurnIntent> GetDeclaration(DuelContext context, CancellationToken ct)
        {
            var tcs = new UniTaskCompletionSource<TurnIntent>();
            
            //ShowUI(context, intent => tcs.TrySetResult(intent));
            // TODO Учим уроки по UniTask
            ct.Register(() => tcs.TrySetCanceled());
            
            return tcs.Task;
        }
        
        public UniTask<TurnIntent> GetTurnIntent(DuelContext context, CancellationToken ct)
        {
            var tcs = new UniTaskCompletionSource<TurnIntent>();
            
            //ShowUI(context, intent => tcs.TrySetResult(intent));
            // TODO Учим уроки по UniTask
            ct.Register(() => tcs.TrySetCanceled());

            return tcs.Task;
        }
    }
}