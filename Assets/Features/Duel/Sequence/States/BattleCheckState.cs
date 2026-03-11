using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;
using Features.Duel.Data.FightEnd;

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
        private FightEndResult fightEndResult;
        
        public void Enter(DuelContext context)
        {
            IFightEndResolver fightEndResolver = modules.Get<IFightEndResolver>();
            fightEndResult = fightEndResolver.Evaluate(context);
        }
        
        public StateResult Tick(DuelContext context)
        {
            if (fightEndResult.IsFightEnded)
            {
                // Зачет победы выигравшему
                foreach (var participant in context.Participants)
                {
                    if (participant.Key == fightEndResult.WinnerId)
                        participant.Value.WinFight();
                }
                
                eventHub.Publish(new BattleEndEvent(context, fightEndResult));
                
                return StateResult.Switch<DuelCheckState>();
            }
            else
            {
                eventHub.Publish(new BattleEndEvent(context, fightEndResult));
                
                return StateResult.Switch<RollPlanningState>();
            }
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}