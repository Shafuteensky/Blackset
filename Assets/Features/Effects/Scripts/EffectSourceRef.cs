namespace Blackset.Effects
{
    /// <summary>
    /// Ссылка на источник эффекта
    /// </summary>
    public sealed class EffectSourceRef
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
        /// Эффект, связанный с источником
        /// </summary>
        public AbstractEffect Effect { get; }

        /// <summary>
        /// Создать ссылку на источник эффекта
        /// </summary>
        /// <param name="sourceInstanceId">Идентификатор предмета из сборки</param>
        /// <param name="sourceKind">Тип источника эффекта</param>
        /// <param name="effect">Эффект источника</param>
        public EffectSourceRef(string sourceInstanceId, EffectSourceKind sourceKind, AbstractEffect effect)
        {
            SourceInstanceId = sourceInstanceId;
            SourceKind = sourceKind;
            Effect = effect;
        }
    }
}