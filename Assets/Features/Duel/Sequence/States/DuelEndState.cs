using Blackset.Duel.Context;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 12. Финал дуэли
    /// </summary>
    /// <remarks>
    /// - Закрытие сессии дуэли, возвращение в меню
    /// </remarks>
    public class DuelEndState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            // TODO Вызов события для показа окна окончания дуэли
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