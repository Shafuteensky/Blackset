using Blackset.Duel.Context;
using Extensions.FiniteStateMachine;

namespace Features.Duel.Sequence.States
{
    /// <summary>
    /// 5. Старт боя дуэли (повторяемое состояние)
    /// </summary>
    /// <remarks>
    /// - Сброс данных текущией дуэли
    /// - Отключение просроченных эффектов
    /// </remarks>
    public class BattleStartState : BaseDuelState, IState<DuelContext>
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