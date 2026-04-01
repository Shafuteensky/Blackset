using Blackset.Data.Registries;
using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;
using Features.Duel.Context;
using UnityEngine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 10. Проверка завершения дуэли (повторяемое состояние)
    /// </summary>
    /// <remarks>
    /// - Завершенеи дуэли если есть победивший по правилам
    /// - Начало нового боя если дуэль не завершена
    /// </remarks>
    public class DuelCheckState : BaseDuelState, IState<DuelContext>
    {
        private DuelEndResult duelEndResult;
        
        public void Enter(DuelContext context)
        {
            IDuelEndResolver duelEndResolver = modules.Get<IDuelEndResolver>();
            duelEndResult = duelEndResolver.Evaluate(context);
        }
        
        public StateResult Tick(DuelContext context)
        {
            if (duelEndResult.IsDuelEnded.Value)
            {
                context.Progress.OnDuelFinished(duelEndResult);
                eventHub.Publish(new DuelFinishEvent(context.Progress.DuelResult));
                return StateResult.Switch<RewardResolveState>();
            }
            else
                return StateResult.Switch<BattleStartState>();
        }
        
        public void Exit(DuelContext context) { }
    }
}