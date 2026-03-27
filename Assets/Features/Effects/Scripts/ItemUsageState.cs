namespace Blackset.Effects
{
    /// <summary>
    /// Данные применения предмета с эффектом в рамках текущего боя
    /// </summary>
    public class ItemUsageState
    {
        /// <summary>
        /// Идентификатор предмета из сборки
        /// </summary>
        public string SourceInstanceId { get; }
        /// <summary>
        /// Тип источника эффекта
        /// </summary>
        public EffectSourceKind SourceKind { get; }

        /// <summary>
        /// Был ли предмет использован в текущем бою
        /// </summary>
        public bool IsUsed { get; private set; }
        /// <summary>
        /// Был ли предмет исчерпан в рамках текущего боя
        /// </summary>
        public bool IsConsumed { get; set; }

        /// <summary>
        /// Номер броска, на котором предмет впервые был использован
        /// </summary>
        public int FirstUsedThrowIndex { get; private set; }
        /// <summary>
        /// Номер броска, на котором эффект последний раз применялся
        /// </summary>
        public int LastAppliedThrowIndex { get; private set; }

        /// <summary>
        /// Сколько раз эффект был применен в текущем бою
        /// </summary>
        public int TimesApplied { get; private set; }

        /// <summary>
        /// Номер броска, на котором эффект должен примениться в следующий раз
        /// </summary>
        public int NextApplyThrowIndex { get; set; }

        /// <summary>
        /// Идентификатор целевого участника применения
        /// </summary>
        public string TargetParticipantId { get; private set; }

        /// <summary>
        /// Создать данные применения предмета
        /// </summary>
        /// <param name="sourceInstanceId">Идентификатор предмета из сборки</param>
        /// <param name="sourceKind">Тип источника эффекта</param>
        public ItemUsageState(string sourceInstanceId, EffectSourceKind sourceKind)
        {
            SourceInstanceId = sourceInstanceId;
            SourceKind = sourceKind;
            
            IsUsed = false;
            IsConsumed = false;
            FirstUsedThrowIndex = -1;
            LastAppliedThrowIndex = -1;
            TimesApplied = 0;
            NextApplyThrowIndex = -1;
            TargetParticipantId = string.Empty;
        }

        /// <summary>
        /// Отметить предмет использованным
        /// </summary>
        /// <param name="throwIndex">Номер текущего броска</param>
        /// <param name="targetParticipantId">Идентификатор целевого участника</param>
        public void MarkUsed(int throwIndex, string targetParticipantId)
        {
            if (!IsUsed)
            {
                IsUsed = true;
                FirstUsedThrowIndex = throwIndex;
            }

            TargetParticipantId = targetParticipantId;
        }

        /// <summary>
        /// Отметить эффект примененным
        /// </summary>
        /// <param name="throwIndex">Номер текущего броска</param>
        public void MarkApplied(int throwIndex)
        {
            LastAppliedThrowIndex = throwIndex;
            TimesApplied += 1;
        }

        /// <summary>
        /// Сбросить состояние применения
        /// </summary>
        public void Reset()
        {
            IsUsed = false;
            IsConsumed = false;
            FirstUsedThrowIndex = -1;
            LastAppliedThrowIndex = -1;
            TimesApplied = 0;
            NextApplyThrowIndex = -1;
            TargetParticipantId = string.Empty;
        }
    }
}