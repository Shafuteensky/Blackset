using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Requests;
using Blackset.Duel.TargetValue;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 4. Определение цели битв дуэли
    /// </summary>
    /// <remarks>
    /// - Определяется целевое значение
    /// - Публикуются условия боя
    /// </remarks>
    public class TargetValueSetupState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            ITargetValueGenerator tvGenerator = modules.Get<ITargetValueGenerator>();
            TargetValueRequest request = new TargetValueRequest(context.Seed, context.Rules, context.Contract.Mode);
            TargetValueContext tvContext = tvGenerator.Generate(request);
            context.TargetValue = tvContext;
            
            eventHub.Publish(new TargetValueSetEvent());
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<BattleStartState>();
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}