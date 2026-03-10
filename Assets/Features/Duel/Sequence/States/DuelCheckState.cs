using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Extensions.FiniteStateMachine;
using Features.Duel.Context;

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
                return StateResult.Switch<RewardResolveState>();
            }
            else
                return StateResult.Switch<BattleStartState>();
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}