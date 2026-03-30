using Blackset.Duel.Context;
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

        public void Enter(DuelContext context) => ApplyEffects(context);

        public StateResult Tick(DuelContext context) => StateResult.Switch<ScoreCommitState>();

        public void Exit(DuelContext context) { }
    }
}