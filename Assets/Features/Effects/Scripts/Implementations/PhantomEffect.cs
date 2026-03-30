using Blackset.Duel.Rolls;
using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект замены текущего результата на модуль разницы с предыдущим своим финальным броском
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PhantomEffect),
        menuName = "Blackset/Effects/" + nameof(PhantomEffect))]
    public class PhantomEffect : AbstractEffect
    {
        public override EffectPhase GetEffectPhase() => EffectPhase.PostRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OnUse;
        
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            if (!context.DuelContext.Participants.TryGetValue(context.OwnerParticipantId, out var participant))
                return false;
            if (!participant.FightState.TryGetLastRoll(out RollHistoryEntry currentRoll))
                return false;
            if (!participant.FightState.TryGetPreviousRoll(out RollHistoryEntry previousRoll))
                return false;

            int ownerDiceMaxValue = EffectsHelpers.GetCurrentDiceMaxValue(context);
            if (ownerDiceMaxValue <= 0)
                return false;

            int diffValue = Mathf.Abs(currentRoll.FinalResult - previousRoll.FinalResult);
            currentRoll.FinalResult = Mathf.Min(diffValue, ownerDiceMaxValue);
            return true;
        }
    }
}