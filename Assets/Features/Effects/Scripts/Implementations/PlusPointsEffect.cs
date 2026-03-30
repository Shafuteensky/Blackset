using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект добавления очков дуэли владельцу
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlusPointsEffect),
        menuName = "Blackset/Effects/" + nameof(PlusPointsEffect))]
    public class PlusPointsEffect : AbstractEffect
    {
        [Header("Количество начисляемых очков дуэли"), Space]
        [SerializeField] private int amount;
        [Header("Шаг на редкость"), Space]
        [SerializeField] private int step;
        
        public override EffectPhase GetEffectPhase() => EffectPhase.PreRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OnUse;
        
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            context.Snapshot.AddDuelScore(context.OwnerParticipantId, amount);
            return true;
        }
    }
}