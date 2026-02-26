using Blackset.Effects;

namespace Features.Effects
{
    /// <summary>
    /// Отложенный эффект (эффект в очереди исполнения по фазам)
    /// </summary>
    public struct DeferredEffect
    {
        /// <summary>
        /// Тип эффекта
        /// </summary>
        public EffectType Type;
        /// <summary>
        /// Фаза применения
        /// </summary>
        public EffectPhase TriggerPhase;
    }
}