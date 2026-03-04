using Blackset.Duel.Pools;
using Blackset.Duel.Requests;
using UnityEngine;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Стандартный билдер пулов участников дуэли
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ParticipantPoolBuilder_Default),
        menuName = "Blackset/Duel/Modules/" + nameof(ParticipantPoolBuilder_Default))]
    public class ParticipantPoolBuilder_Default : BaseDuelModule, IParticipantPoolBuilder
    {
        public DuelPoolsContext BuildPools(PoolBuildRequest request)
        {
            DuelPoolsContext poolsContext = new(request.DicesPool, request.ConsumablesPool);
            return poolsContext;
        }
    }
}