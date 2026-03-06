using Blackset.Duel.Context;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 9. Проверка завершения боя (повторяемое состояние)
    /// </summary>
    /// <remarks>
    /// - Обновление данных о потенциальных победителе и проигравшем
    /// - Завершение боя при окончании боя по правилам или авто-победе/поражении
    /// </remarks>
    public class BattleCheckState : BaseDuelState, IState<DuelContext>
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