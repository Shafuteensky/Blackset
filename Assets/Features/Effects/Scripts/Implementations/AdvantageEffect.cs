using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект преимущества на текущий бросок
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(AdvantageEffect),
        menuName = "Blackset/Effects/" + nameof(AdvantageEffect))]
    public class AdvantageEffect : AbstractEffect
    {
        public override EffectPhase GetEffectPhase() => EffectPhase.PreRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OnUse;

        protected override bool ApplyInternal(EffectApplyContext context)
        {
            if (!TryGetTargetParticipant(context, out var participant))
                return false;

            participant.FightState.TurnState.RollModifiers.SetAdvantage();
            return true;
        }
    }
}