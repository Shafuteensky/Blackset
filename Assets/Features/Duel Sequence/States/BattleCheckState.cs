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
    public class BattleCheckState : BaseDuelState, IState<DuelContext>
    {
        private FightEndResult fightEndResult;
        private IDuelScoreResolver duelScoreResolver;
        
        public void Enter(DuelContext context)
        {
            IFightEndResolver fightEndResolver = modules.Get<IFightEndResolver>();
            fightEndResult = fightEndResolver.Evaluate(context);

            duelScoreResolver = modules.Get<IDuelScoreResolver>();

            duelScoreResolver.ResolveDuelWin(context, fightEndResult);
        }
        
        public StateResult Tick(DuelContext context)
        {
            if (fightEndResult.IsFightEnded)
            {
                foreach (var participant in context.Participants)
                {
                    if (participant.Key == fightEndResult.WinnerId)
                    {
                        participant.Value.WinFight();
                    }
                }

                duelScoreResolver.ResolveUnusedItems(context);
                
                eventHub.Publish(new BattleEndEvent(context, fightEndResult));
                return StateResult.Switch<DuelCheckState>();
            }

            eventHub.Publish(new BattleEndEvent(context, fightEndResult));
            return StateResult.Switch<RollPlanningState>();
        }
        
        public void Exit(DuelContext context) { }
    }
}