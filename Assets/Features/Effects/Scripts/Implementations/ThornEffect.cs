using Blackset.Duel.Rolls;
using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект отбирания очков счета соперника в количестве от результата текущего броска
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ThornEffect),
        menuName = "Blackset/Effects/" + nameof(ThornEffect))]
    public class ThornEffect : AbstractEffect
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

            int amount = currentRoll.RawResult;
            counterpartParticipant.FightState.PersistentFightScoreModifier.Value -= amount;

            currentRoll.FinalResult = 0;
            return true;
        }
    }
}