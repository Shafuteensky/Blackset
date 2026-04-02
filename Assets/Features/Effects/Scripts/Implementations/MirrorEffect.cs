using Blackset.Duel.Rolls;
using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект копирования последнего финального броска соперника (не выше макс. дайса)
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(MirrorEffect),
        menuName = "Blackset/Effects/" + nameof(MirrorEffect))]
    public class MirrorEffect : AbstractEffect
    {
        public override EffectPhase GetEffectPhase() => EffectPhase.PostRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OnUse;
        
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            if (!TryGetTargetParticipant(context, out var participant))
                return false;
            if (!participant.FightState.TryGetLastRoll(out RollHistoryEntry currentRoll))
                return false;

            string counterpartId = GetCounterpartId(context, context.TargetParticipantId);
            if (string.IsNullOrEmpty(counterpartId))
                return false;
            if (!context.DuelContext.Participants.TryGetValue(counterpartId, out var counterpartParticipant))
                return false;
            if (!counterpartParticipant.FightState.TryGetPreviousRoll(out RollHistoryEntry counterpartPreviousRoll))
                return false;

            int targetDiceMaxValue = EffectsHelpers.GetTargetCurrentDiceMaxValue(context);
            if (targetDiceMaxValue <= 0)
                return false;

            currentRoll.FinalResult = Mathf.Min(counterpartPreviousRoll.FinalResult, targetDiceMaxValue);
            return true;
        }
    }
}