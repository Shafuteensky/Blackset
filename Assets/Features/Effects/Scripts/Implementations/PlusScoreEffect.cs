using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект добавления очков битвы владельцу
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlusScoreEffect),
        menuName = "Blackset/Effects/" + nameof(PlusScoreEffect))]
    public class PlusScoreEffect : AbstractEffect
    {
        [Header("Количество начисляемых очков битвы"), Space]
        [SerializeField] private int amount;
        [Header("Шаг на редкость"), Space]
        [SerializeField] private int step;
        
        public override EffectPhase GetEffectPhase() => EffectPhase.PreRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OncePerBattle;
        
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            context.Snapshot.AddScore(context.OwnerParticipantId, amount);
            return true;
        }
    }
}