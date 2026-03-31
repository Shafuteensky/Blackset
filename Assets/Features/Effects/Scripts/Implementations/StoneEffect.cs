using Blackset.Duel.Rolls;
using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект ограничения минимального результата следующего броска
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(StoneEffect),
        menuName = "Blackset/Effects/" + nameof(StoneEffect))]
    public class StoneEffect : AbstractEffect
    {
        [Header("Доля от математического ожидания дайса"), Space]
        [SerializeField] private float expectedValueFactor = 0.25f;

        public override EffectPhase GetEffectPhase() => EffectPhase.PostRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OnUse;

        protected override bool ApplyInternal(EffectApplyContext context)
        {
            if (!context.DuelContext.Participants.TryGetValue(context.OwnerParticipantId, out var participant))
                return false;
            if (!participant.FightState.TryGetLastRoll(out RollHistoryEntry currentRoll))
                return false;

            int minValue = GetCurrentDiceExpectedClamp(context, expectedValueFactor);
            int maxValue = EffectsHelpers.GetCurrentDiceMaxValue(context);

            currentRoll.FinalResult = Mathf.Min(
                Mathf.Max(currentRoll.FinalResult, minValue),
                maxValue);

            return true;
        }

        public static int GetCurrentDiceExpectedClamp(EffectApplyContext context, float expectedValueFactor)
        {
            int[] sideNumbers = EffectsHelpers.GetCurrentDiceSideNumbers(context);
            if (sideNumbers == null || sideNumbers.Length == 0) return 0;

            float sum = 0f;
            foreach (int value in sideNumbers)
                sum += value;

            float expectedValue = sum / sideNumbers.Length;
            return Mathf.Max(1, Mathf.CeilToInt(expectedValue * expectedValueFactor));
        }
    }
}