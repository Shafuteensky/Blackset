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
    /// - Новый ход при продолжении боя
    /// </remarks>
    public class BattleCheckState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            
        }
        
        public StateResult Tick(DuelContext context)
        {
            if (true)
                return StateResult.Switch<DuelCheckState>();
            else
                return StateResult.Switch<RollPlanningState>();
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}