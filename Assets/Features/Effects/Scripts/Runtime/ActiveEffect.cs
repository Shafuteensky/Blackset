using System.Collections.Generic;
using Blackset.Effects;

namespace Features.Effects
{
    /// <summary>
    /// Активный эффект
    /// </summary>
    /// <remarks>
    /// Никак не связан с отложенными эффектами: этот считает эффектом длительного воздействия, а не разового как DeferredEffect
    /// </remarks>
    public class ActiveEffect
    {
        public DeferredEffectsQueue EffectsQueues { get; }
        public string TargetId { get; }
        public int ThrowsDuration { get; }
        public EffectPhase TriggerPhase { get; }
        
        //public ActiveEffect(List<string> participantsIds, string targetId, int throwsDuration, EffectPhase triggerPhase)
    }
}