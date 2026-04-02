using Blackset.Duel.Participants;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Базовый абстрактный эффект
    /// </summary>
    public abstract class AbstractEffect : ScriptableObject
    {
        /// <summary>
        /// Попытаться применить эффект
        /// </summary>
        /// <param name="context">Контекст применения</param>
        /// <returns>true если эффект был применен, иначе false</returns>
        public bool TryApplyEffect(EffectApplyContext context)
        {
            if (context.CurrentPhase != GetEffectPhase() || !CanApplyByPolicy(context)) return false;

            bool applied = ApplyInternal(context);
            if (applied) context.UsageState.MarkApplied(context.ThrowIndex);

            return applied;
        }

        /// <summary>
        /// Фаза применения эффекта
        /// </summary>
        public abstract EffectPhase GetEffectPhase();
        /// <summary>
        /// Политика применения эффекта
        /// </summary>
        public abstract EffectApplyPolicy GetApplyPolicy();
        
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

            bool isSelectedNow =
                context.SourceKind == EffectSourceKind.Dice
                    ? context.SelectedDiceInstanceId == context.SourceInstanceId
                    : context.SelectedConsumableInstanceId == context.SourceInstanceId;

            switch (GetApplyPolicy())
            {
                case EffectApplyPolicy.OnUse:
                    return isSelectedNow &&
                           !usage.IsConsumed &&
                           usage.LastAppliedThrowIndex != context.ThrowIndex;

                case EffectApplyPolicy.OnNextThrow:
                    return !usage.IsConsumed &&
                           usage.NextApplyThrowIndex == context.ThrowIndex &&
                           usage.LastAppliedThrowIndex != context.ThrowIndex;

                case EffectApplyPolicy.EveryThrowWhileUsed:
                    return usage.IsUsed &&
                           !usage.IsConsumed &&
                           usage.LastAppliedThrowIndex != context.ThrowIndex &&
                           context.ThrowIndex >= usage.FirstUsedThrowIndex;

                case EffectApplyPolicy.OncePerBattle:
                    return isSelectedNow &&
                           !usage.IsConsumed &&
                           usage.TimesApplied == 0;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Получить идентификатор соперника владельца эффекта
        /// </summary>
        protected string GetOpponentId(EffectApplyContext context)
        {
            foreach (string participantId in context.DuelContext.Participants.Keys)
            {
                if (participantId != context.OwnerParticipantId)
                    return participantId;
            }

            return string.Empty;
        }

        protected string GetCounterpartId(EffectApplyContext context, string participantId)
        {
            foreach (string currentParticipantId in context.DuelContext.Participants.Keys)
            {
                if (currentParticipantId != participantId)
                    return currentParticipantId;
            }

            return string.Empty;
        }
        
        protected bool TryGetOwnerParticipant(EffectApplyContext context, out DuelParticipantState participant)
        {
            return context.DuelContext.Participants.TryGetValue(context.OwnerParticipantId, out participant);
        }

        protected bool TryGetTargetParticipant(EffectApplyContext context, out DuelParticipantState participant)
        {
            return context.DuelContext.Participants.TryGetValue(context.TargetParticipantId, out participant);
        }
    }
}