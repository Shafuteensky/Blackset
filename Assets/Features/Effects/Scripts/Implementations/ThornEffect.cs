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
            if (!context.DuelContext.Participants.TryGetValue(context.OwnerParticipantId, out var participant))
                return false;
            if (!participant.FightState.TryGetLastRoll(out RollHistoryEntry currentRoll))
                return false;
            
            string opponentId = GetOpponentId(context);
            int amount = currentRoll.RawResult;
            context.DuelContext.Participants[opponentId].FightState.PersistentFightScoreModifier.Value -= amount;


            currentRoll.FinalResult = 0;
            
            return true;
        }
        
    }
}