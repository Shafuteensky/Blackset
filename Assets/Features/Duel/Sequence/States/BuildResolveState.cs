using Blackset.Duel.Context;
using Extensions.FiniteStateMachine;

namespace Features.Duel.Sequence.States
{
    /// <summary>
    /// 
    /// </summary>
    public class BuildResolveState : IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            
        }
        
        public StateResult Tick(DuelContext context)
        {
            return new StateResult();
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}