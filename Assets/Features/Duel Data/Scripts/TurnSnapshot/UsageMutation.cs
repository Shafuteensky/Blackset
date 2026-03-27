using Blackset.Effects;

namespace Blackset.Duel.Snapshots
{
    /// <summary>
    /// Изменение состояния использования источника эффекта по итогам текущего броска
    /// </summary>
    public sealed class UsageMutation
    {
        /// <summary>
        /// Идентификатор владельца источника
        /// </summary>
        public string OwnerParticipantId { get; }
        /// <summary>
        /// Идентификатор источника из сборки
        /// </summary>
        public string SourceInstanceId { get; }

        /// <summary>
        /// Тип источника
        /// </summary>
        public EffectSourceKind SourceKind { get; }

        /// <summary>
        /// Идентификатор цели применения
        /// </summary>
        public string TargetParticipantId { get; }

        /// <summary>
        /// Создать запись изменения использования
        /// </summary>
        public UsageMutation(
            string ownerParticipantId,
            string sourceInstanceId,
            EffectSourceKind sourceKind,
            string targetParticipantId)
        {
            OwnerParticipantId = ownerParticipantId;
            SourceInstanceId = sourceInstanceId;
            SourceKind = sourceKind;
            TargetParticipantId = targetParticipantId;
        }
    }
}