using Blackset.Duel.Requests;
using Blackset.Duel.Sets;
using UnityEngine;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Случайный генератор сборок участников дуэли
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ParticipantSetGenerator_Random),
        menuName = "Blackset/Duel/Modules/" + nameof(ParticipantSetGenerator_Random))]
    public class ParticipantSetGenerator_Random : BaseDuelModule, IParticipantSetGenerator
    {
        public DuelSetsContext GenerateSets(SetGenerationRequest request)
        {
            DuelSetsContext setsContext = default;

            // setsContext = new();
            
            return setsContext;
        }
    }
}