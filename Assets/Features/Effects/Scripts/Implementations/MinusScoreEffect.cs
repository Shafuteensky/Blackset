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

        /// <summary>
        /// Применить внутреннюю логику эффекта
        /// </summary>
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            context.Snapshot.AddScore(context.OwnerParticipantId, -amount);
            return true;
        }
    }
}