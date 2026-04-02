using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект добавления очков дуэли владельцу
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlusDuelScoreEffect),
        menuName = "Blackset/Effects/" + nameof(PlusDuelScoreEffect))]
    public class PlusDuelScoreEffect : AbstractEffect
    {
        [Header("Количество начисляемых очков дуэли"), Space]
        [SerializeField] private int amount;
        [Header("Шаг на редкость"), Space]
        [SerializeField] private int step;
        
        public override EffectPhase GetEffectPhase() => EffectPhase.PreRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OncePerBattle;
        
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            if (!context.DuelContext.Participants.TryGetValue(context.TargetParticipantId, out var participant))
                return false;

            participant.AddDuelScore(amount);
            return true;
        }
    }
}