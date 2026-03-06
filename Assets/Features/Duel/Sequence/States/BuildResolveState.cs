using Blackset.Duel.Context;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 3. Фиксация сборок участников
    /// </summary>
    /// <remarks>
    /// - Генерируются сборки участников согласно правилам
    /// </remarks>
    public class BuildResolveState : BaseDuelState, IState<DuelContext>
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