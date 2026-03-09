using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
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
            context.Progress.OnNewFight();
            foreach (DuelParticipantState participant in context.Participants.Values)
            {
                participant.FightState.ResetForNewFight();
            }
            // TODO Работа с эффектами
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<RollPlanningState>();
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}