using Blackset.Duel.Context;
using Blackset.Duel.Rolls;
using Blackset.Duel.Snapshots;
using Blackset.Effects;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// Применение эффектов фазы PostRoll
    /// </summary>
    public class PostRollEffectState : BaseEffectState, IState<DuelContext>
    {
        protected override EffectPhase Phase => EffectPhase.PostRoll;

        protected override TurnSnapshot ResolveSnapshot(DuelContext context)
            => context.Progress.CurrentTurnSnapshot; // null → ApplyEffects сразу вернётся

        protected override bool ShouldSkipParticipant(string participantId, TurnSnapshot snapshot)
            => !snapshot.TryGetCurrentRoll(participantId, out RollHistoryEntry _);

        public void Enter(DuelContext context) => ApplyEffects(context);

        public StateResult Tick(DuelContext context) => StateResult.Switch<ScoreCommitState>();

        public void Exit(DuelContext context) { }
    }
}