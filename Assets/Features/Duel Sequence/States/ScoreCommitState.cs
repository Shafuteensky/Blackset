using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Snapshots;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 8. Коммит рабочего снапшота текущего броска
    /// </summary>
    public class ScoreCommitState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            TurnSnapshot snapshot = context.Progress.CurrentTurnSnapshot;
            if (snapshot == null) return;

            ISnapshotCommiter commiterDefault = modules.Get<ISnapshotCommiter>();
            commiterDefault.Commit(snapshot, context);

            context.Progress.CurrentTurnSnapshot = null;
            eventHub.Publish(new EffectsResolvedEvent(context));
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<BattleCheckState>();
        }
        
        public void Exit(DuelContext context) { }
    }
}