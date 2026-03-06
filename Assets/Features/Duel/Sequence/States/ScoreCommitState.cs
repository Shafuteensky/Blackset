using Blackset.Duel.Context;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 8. Применение расчетов (повторяемое состояние)
    /// </summary>
    /// <remarks>
    /// - Обновление фактических счетов
    /// - Запись в историю
    /// </remarks>
    public class ScoreCommitState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<BattleCheckState>();
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}