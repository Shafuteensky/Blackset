using Blackset.Duel.Snapshots;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Стандартный резолвер эффектов
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(EffectsResolutionPipeline_Default),
        menuName = "Blackset/Duel/Modules/" + nameof(EffectsResolutionPipeline_Default))]
    public class EffectsResolutionPipeline_Default : BaseDuelModule, IEffectsResolutionPipeline
    {
        public TurnSnapshot Resolve(TurnSnapshot snapshot)
        {
            TurnSnapshot resolvedSnapshot = snapshot;
            
            // TODO Завершить по готовности системы эффектов
            
            return resolvedSnapshot;
        }
    }
}