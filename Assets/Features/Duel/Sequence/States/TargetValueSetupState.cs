using Blackset.Duel.Context;
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