using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект убавления очков дуэли владельцу
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(MinusDuelScoreEffect),
        menuName = "Blackset/Effects/" + nameof(MinusDuelScoreEffect))]
    public class MinusDuelScoreEffect : AbstractEffect
    {
        [Header("Количество убавляемых очков дуэли"), Space]
        [SerializeField] private int amount;
        [Header("Шаг на редкость"), Space]
        [SerializeField] private int step;
        
        public override EffectPhase GetEffectPhase() => EffectPhase.PreRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OncePerBattle;
        
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            if (!TryGetTargetParticipant(context, out var participant))
                return false;

            participant.AddDuelScore(-amount);
            return true;
        }
    }
}