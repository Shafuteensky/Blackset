using Blackset.Duel.Context;
using Extensions.FiniteStateMachine;

namespace Features.Duel.Sequence.States
{
    /// <summary>
    /// 2. Подготовка билдов игроков
    /// </summary>
    /// <remarks>
    /// - Сборка пулов участников
    /// - Применение правил, связанных с пулами
    /// </remarks>
    public class BuildPreparationState : BaseDuelState, IState<DuelContext>
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