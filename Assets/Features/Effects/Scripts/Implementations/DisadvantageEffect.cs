using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект помехи на текущий бросок
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(DisadvantageEffect),
        menuName = "Blackset/Effects/" + nameof(DisadvantageEffect))]
    public class DisadvantageEffect : AbstractEffect
    {
        public override EffectPhase GetEffectPhase() => EffectPhase.PreRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OnUse;

        protected override bool ApplyInternal(EffectApplyContext context)
        {
            if (!TryGetTargetParticipant(context, out var participant))
                return false;

            participant.FightState.TurnState.RollModifiers.SetDisadvantage();
            return true;
        }
    }
}