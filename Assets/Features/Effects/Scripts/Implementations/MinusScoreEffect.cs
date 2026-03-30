using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект снятия очков битвы у владельца
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(MinusScoreEffect),
        menuName = "Blackset/Effects/" + nameof(MinusScoreEffect))]
    public class MinusScoreEffect : AbstractEffect
    {
        [Header("Количество убавляемых очков битвы"), Space]
        [SerializeField] private int amount;
        [Header("Шаг на редкость"), Space]
        [SerializeField] private int step;

        public override EffectPhase GetEffectPhase() => EffectPhase.PreRoll;
        public override EffectApplyPolicy GetApplyPolicy() => EffectApplyPolicy.OncePerBattle;
        
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            context.DuelContext.Participants[context.OwnerParticipantId].FightState.AddScore(-amount);
            return true;
        }
    }
}