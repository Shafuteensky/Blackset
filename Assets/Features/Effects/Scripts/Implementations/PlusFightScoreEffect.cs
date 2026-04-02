using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект добавления очков битвы владельцу
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlusFightScoreEffect),
        menuName = "Blackset/Effects/" + nameof(PlusFightScoreEffect))]
    public class PlusFightScoreEffect : AbstractEffect
    {
        [Header("Количество начисляемых очков битвы"), Space]
        [SerializeField] private int amount;
        [Header("Шаг на редкость"), Space]
        [SerializeField] private int step;
        
        public override EffectPhase GetEffectPhase() => EffectPhase.PreRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OncePerBattle;
        
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            if (!TryGetTargetParticipant(context, out var participant))
                return false;

            participant.FightState.PersistentFightScoreModifier.Value += amount;
            return true;
        }
    }
}