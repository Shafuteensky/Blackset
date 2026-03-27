using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Базовый абстрактный эффект
    /// </summary>
    public abstract class AbstractEffect : ScriptableObject
    {
        /// <summary>
        /// Политика применения эффекта
        /// </summary>
        public EffectApplyPolicy ApplyPolicy => applyPolicy;
        /// <summary>
        /// Фаза применения эффекта
        /// </summary>
        public EffectPhase EffectPhase => effectPhase;

        [SerializeField] protected EffectApplyPolicy applyPolicy;
        [SerializeField] protected EffectPhase effectPhase;

        /// <summary>
        /// Попытаться применить эффект
        /// </summary>
        /// <param name="context">Контекст применения</param>
        /// <returns>true если эффект был применен, иначе false</returns>
        public bool TryApplyEffect(EffectApplyContext context)
        {
            if (context.CurrentPhase != effectPhase || !CanApplyByPolicy(context)) return false;

            bool applied = ApplyInternal(context);
            if (applied) context.UsageState.MarkApplied(context.ThrowIndex);
            return applied;
        }

        /// <summary>
        /// Применить внутреннюю логику эффекта
        /// </summary>
        /// <param name="context">Контекст применения</param>
        /// <returns>true если эффект был применен, иначе false</returns>
        protected abstract bool ApplyInternal(EffectApplyContext context);

        /// <summary>
        /// Проверить, разрешено ли применение эффекта по политике применения
        /// </summary>
        /// <param name="context">Контекст применения</param>
        /// <returns>true если применение разрешено</returns>
        protected virtual bool CanApplyByPolicy(EffectApplyContext context)
        {
            ItemUsageState usage = context.UsageState;

            switch (applyPolicy)
            {
                case EffectApplyPolicy.OnUse:
                    return usage.IsUsed &&
                           usage.FirstUsedThrowIndex == context.ThrowIndex &&
                           usage.TimesApplied == 0 &&
                           !usage.IsConsumed;

                case EffectApplyPolicy.OnNextThrow:
                    return usage.IsUsed &&
                           usage.NextApplyThrowIndex == context.ThrowIndex &&
                           !usage.IsConsumed;

                case EffectApplyPolicy.EveryThrowWhileUsed:
                    return usage.IsUsed &&
                           !usage.IsConsumed &&
                           usage.LastAppliedThrowIndex != context.ThrowIndex &&
                           context.ThrowIndex >= usage.FirstUsedThrowIndex;

                case EffectApplyPolicy.OncePerBattle:
                    return usage.IsUsed &&
                           !usage.IsConsumed &&
                           usage.TimesApplied == 0;

                default:
                    return false;
            }
        }
    }
}