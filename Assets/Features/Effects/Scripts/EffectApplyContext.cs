using Blackset.Duel.Context;
using Blackset.Duel.Snapshots;

namespace Blackset.Effects
{
    /// <summary>
    /// Контекст применения эффекта
    /// </summary>
    public sealed class EffectApplyContext
    {
        /// <summary>
        /// Контекст дуэли
        /// </summary>
        public DuelContext DuelContext { get; }

        /// <summary>
        /// Текущий снапшот броска/фазы
        /// </summary>
        public TurnSnapshot Snapshot { get; }

        /// <summary>
        /// Текущая фаза применения
        /// </summary>
        public EffectPhase CurrentPhase { get; }

        /// <summary>
        /// Номер текущего броска в бою
        /// </summary>
        public int ThrowIndex { get; }

        /// <summary>
        /// Идентификатор владельца эффекта
        /// </summary>
        public string OwnerParticipantId { get; }

        /// <summary>
        /// Идентификатор цели применения эффекта
        /// </summary>
        public string TargetParticipantId { get; }

        /// <summary>
        /// Идентификатор источника эффекта
        /// </summary>
        public string SourceInstanceId { get; }

        /// <summary>
        /// Тип источника эффекта
        /// </summary>
        public EffectSourceKind SourceKind { get; }

        /// <summary>
        /// Идентификатор выбранного дайса в текущем броске
        /// </summary>
        public string SelectedDiceInstanceId { get; }

        /// <summary>
        /// Идентификатор выбранного расходника в текущем броске
        /// </summary>
        public string SelectedConsumableInstanceId { get; }

        /// <summary>
        /// Состояние применения предмета
        /// </summary>
        public ItemUsageState UsageState { get; }

        /// <summary>
        /// Создать контекст применения эффекта
        /// </summary>
        public EffectApplyContext(
            DuelContext duelContext,
            TurnSnapshot snapshot,
            EffectPhase currentPhase,
            int throwIndex,
            string ownerParticipantId,
            string targetParticipantId,
            string sourceInstanceId,
            EffectSourceKind sourceKind,
            string selectedDiceInstanceId,
            string selectedConsumableInstanceId,
            ItemUsageState usageState)
        {
            DuelContext = duelContext;
            Snapshot = snapshot;
            CurrentPhase = currentPhase;
            ThrowIndex = throwIndex;
            OwnerParticipantId = ownerParticipantId;
            TargetParticipantId = targetParticipantId;
            SourceInstanceId = sourceInstanceId;
            SourceKind = sourceKind;
            SelectedDiceInstanceId = selectedDiceInstanceId;
            SelectedConsumableInstanceId = selectedConsumableInstanceId;
            UsageState = usageState;
        }
    }
}