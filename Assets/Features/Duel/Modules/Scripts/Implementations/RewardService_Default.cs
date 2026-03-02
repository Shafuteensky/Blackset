using Blackset.Duel.Requests;
using UnityEngine;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Стандартный билдер награды на дуэль
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(RewardService_Default),
        menuName = "Blackset/Duel/Modules/" + nameof(RewardService_Default))]
    public class RewardService_Default : BaseDuelModule, IRewardService
    {
        public DuelRewards BuildReward(RewardRequest request)
        {
            DuelRewards result = default; 
            
            // result = new DuelRewards();
            
            return result;
        }

        public void ApplyResult(DuelRewards result)
        {
            // Обновляем PlayerMetaData и инвентари 
        }
    }
}