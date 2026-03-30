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
            if (!context.DuelContext.Participants.TryGetValue(context.OwnerParticipantId, out var participant))
                return false;
            if (!participant.FightState.TryGetLastRoll(out RollHistoryEntry currentRoll))
                return false;
            
            string opponentId = ResolveOpponentId(context);
            if (string.IsNullOrEmpty(opponentId))
                return false;

            if (!context.DuelContext.Participants.TryGetValue(opponentId, out var opponentParticipant))
                return false;
            if (!opponentParticipant.FightState.TryGetLastRoll(out RollHistoryEntry opponentLastRoll))
                return false;

            int ownerDiceMaxValue = EffectsHelpers.GetCurrentDiceMaxValue(context);
            if (ownerDiceMaxValue <= 0)
                return false;

            currentRoll.FinalResult = Mathf.Min(opponentLastRoll.FinalResult, ownerDiceMaxValue);
            return true;
        }
    }
}