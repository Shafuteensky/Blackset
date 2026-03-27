using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Snapshots;
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

            TurnSnapshot snapshot = new TurnSnapshot(context);
            duelScoreResolver.ResolveDuelWin(context, fightEndResult, snapshot);
            ApplyDuelScores(snapshot, context);
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

                TurnSnapshot snapshot = new TurnSnapshot(context);
                duelScoreResolver.ResolveUnusedItems(context, snapshot);
                ApplyDuelScores(snapshot, context);
                
                eventHub.Publish(new BattleEndEvent(context, fightEndResult));
                return StateResult.Switch<DuelCheckState>();
            }

            eventHub.Publish(new BattleEndEvent(context, fightEndResult));
            return StateResult.Switch<RollPlanningState>();
        }
        
        public void Exit(DuelContext context)
        {
        }

        private void ApplyDuelScores(TurnSnapshot snapshot, DuelContext context)
        {
            foreach (var pair in context.Participants)
            {
                if (snapshot.ParticipantDuelScores.TryGetValue(pair.Key, out int duelScore))
                {
                    pair.Value.DuelScore.Value = duelScore;
                }
            }
        }
    }
}