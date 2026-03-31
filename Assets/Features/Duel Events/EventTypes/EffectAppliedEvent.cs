using Blackset.Effects;

namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Эффект успешно применен
    /// </summary>
    public sealed class EffectAppliedEvent
    {
        /// <summary>
        /// Идентификатор владельца эффекта
        /// </summary>
        public string OwnerParticipantId { get; }
        /// <summary>
        /// Идентификатор цели эффекта
        /// </summary>
        public string TargetParticipantId { get; }
        /// <summary>
        /// Идентификатор источника эффекта
        /// </summary>
        public string SourceInstanceId { get; }
        /// <summary>
        /// Фаза применения эффекта
        /// </summary>
        public EffectPhase Phase { get; }
        /// <summary>
        /// Имя типа эффекта
        /// </summary>
        public string EffectName { get; }

        public EffectAppliedEvent(
            string ownerParticipantId,
            string targetParticipantId,
            string sourceInstanceId,
            EffectPhase phase,
            string effectName)
        {
            OwnerParticipantId = ownerParticipantId;
            TargetParticipantId = targetParticipantId;
            SourceInstanceId = sourceInstanceId;
            Phase = phase;
            EffectName = effectName;
        }
    }
}