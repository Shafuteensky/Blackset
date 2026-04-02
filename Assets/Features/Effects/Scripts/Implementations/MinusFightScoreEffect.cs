using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект снятия очков битвы у владельца
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(MinusFightScoreEffect),
        menuName = "Blackset/Effects/" + nameof(MinusFightScoreEffect))]
    public class MinusFightScoreEffect : AbstractEffect
    {
        [Header("Количество убавляемых очков битвы"), Space]
        [SerializeField] private int amount;
        [Header("Шаг на редкость"), Space]
        [SerializeField] private int step;

        public override EffectPhase GetEffectPhase() => EffectPhase.PreRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OncePerBattle;
        
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            if (!context.DuelContext.Participants.TryGetValue(context.TargetParticipantId, out var participant))
                return false;

            participant.FightState.PersistentFightScoreModifier.Value -= amount;
            return true;
        }
    }
}