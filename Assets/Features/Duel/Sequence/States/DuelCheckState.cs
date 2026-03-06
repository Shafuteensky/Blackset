using Blackset.Duel.Context;
using Extensions.FiniteStateMachine;

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