using Blackset.Duel.Context;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 11. Расчет и выдача наградд
    /// </summary>
    /// <remarks>
    /// - Формирование награды по контракту с учетом бонусов и правил
    /// - Применение наград к инвентарю игрока и его мета-прогрессу
    /// </remarks>
    public class RewardResolveState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<DuelEndState>();
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}