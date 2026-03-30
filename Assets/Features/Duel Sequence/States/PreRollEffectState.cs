using Blackset.Duel.Context;
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

        public void Enter(DuelContext context) => ApplyEffects(context);

        public StateResult Tick(DuelContext context) => StateResult.Switch<RollResolveState>();

        public void Exit(DuelContext context) { }
    }
}