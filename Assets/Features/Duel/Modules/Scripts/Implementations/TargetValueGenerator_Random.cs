using Blackset.Duel.Requests;
using Blackset.Duel.TargetValue;
using UnityEngine;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Случайный генератор целевого значения
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(TargetValueGenerator_Random),
        menuName = "Blackset/Duel/Modules/" + nameof(TargetValueGenerator_Random))]
    public class TargetValueGenerator_Random : BaseDuelModule, ITargetValueGenerator
    {
        public TargetValueContext Generate(TargetValueRequest request)
        {
            TargetValueContext targetValue = default;
            
            // targetValue = new();
            
            return targetValue;
        }
    }
}