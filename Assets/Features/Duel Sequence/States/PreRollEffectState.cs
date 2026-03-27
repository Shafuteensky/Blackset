using Blackset.Duel.Context;
using Blackset.Duel.Snapshots;
using Blackset.Effects;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// Применение эффектов фазы PreRoll
    /// </summary>
    public class PreRollEffectState : BaseEffectState, IState<DuelContext>
    {
        protected override EffectPhase Phase => EffectPhase.PreRoll;

        protected override TurnSnapshot ResolveSnapshot(DuelContext context)
        {
            if (context.Progress.CurrentTurnSnapshot == null)
                context.Progress.CurrentTurnSnapshot = new TurnSnapshot(context);

            return context.Progress.CurrentTurnSnapshot;
        }

        protected override bool ShouldSkipParticipant(string participantId, TurnSnapshot snapshot) => false;
        
        public void Enter(DuelContext context) => ApplyEffects(context);

        public StateResult Tick(DuelContext context) => StateResult.Switch<RollResolveState>();

        public void Exit(DuelContext context) { }
    }
}