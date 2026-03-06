using Blackset.Duel.Context;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 7. Исполнение выборов (повторяемое состояние)
    /// </summary>
    /// <remarks>
    /// - Построение снапшота хода боя
    /// - Генерация результатов бросков
    /// - Резовл снапшота: расчеты по правилам с учетом штормов и эффектов
    /// </remarks>
    public class RollResolveState : BaseDuelState, IState<DuelContext>
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