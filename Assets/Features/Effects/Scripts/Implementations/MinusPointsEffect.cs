using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект убавления очков дуэли владельцу
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(MinusPointsEffect),
        menuName = "Blackset/Effects/" + nameof(MinusPointsEffect))]
    public class MinusPointsEffect : AbstractEffect
    {
        [Header("Количество убавляемых очков дуэли"), Space]
        [SerializeField] private int amount;
        [Header("Шаг на редкость"), Space]
        [SerializeField] private int step;
        
        /// <summary>
        /// Применить внутреннюю логику эффекта
        /// </summary>
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            context.Snapshot.AddDuelScore(context.OwnerParticipantId, -amount);
            return true;
        }
    }
}