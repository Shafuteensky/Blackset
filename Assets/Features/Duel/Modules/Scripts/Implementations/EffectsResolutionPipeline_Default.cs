using Blackset.Duel.Snapshots;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Стандартный резолвер эффектов и роллов
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(EffectsResolutionPipeline_Default),
        menuName = "Blackset/Duel/Modules/" + nameof(EffectsResolutionPipeline_Default))]
    public class EffectsResolutionPipeline_Default : BaseDuelModule, IEffectsResolutionPipeline
    {
        public TurnSnapshot Resolve(TurnSnapshot snapshot)
        {
            // TODO Применение snapshot.RawRolls к счетам:
            
            // foreach (var participant in resolvedSnapshot.ParticipantStates)
            // {
            //     participant.Value.
            // }
            // foreach (int rollResult in pair.Value.FightState.RawRollResults.Values)
            // {
            //     snapshotScore += rollResult;
            // }
            
            // TODO Завершить по готовности системы эффектов
            
            TurnSnapshot resolvedSnapshot = snapshot;
            
            return resolvedSnapshot;
        }
    }
}