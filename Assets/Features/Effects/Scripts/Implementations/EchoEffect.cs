using Blackset.Duel.Rolls;
using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект выбора лучшего между текущим и предыдущим своим финальным броском (не выше макс. дайса)
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(EchoEffect),
        menuName = "Blackset/Effects/" + nameof(EchoEffect))]
    public class EchoEffect : AbstractEffect
    {
        public override EffectPhase GetEffectPhase() => EffectPhase.PostRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OnUse;
        
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            if (!TryGetTargetParticipant(context, out var participant))
                return false;
            if (!participant.FightState.TryGetLastRoll(out RollHistoryEntry currentRoll))
                return false;
            if (!participant.FightState.TryGetPreviousRoll(out RollHistoryEntry previousRoll))
                return false;

            int targetDiceMaxValue = EffectsHelpers.GetTargetCurrentDiceMaxValue(context);
            if (targetDiceMaxValue <= 0)
                return false;

            int bestValue = Mathf.Max(currentRoll.FinalResult, previousRoll.FinalResult);
            currentRoll.FinalResult = Mathf.Min(bestValue, targetDiceMaxValue);
            return true;
        }
    }
}