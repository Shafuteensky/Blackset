using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Registries;
using Blackset.Duel.Requests;
using Blackset.Inventories.Cells;
using Blackset.Opponents;
using UnityEngine;

namespace Blackset.Duel.Modules
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
            // Опыт
            GameData gameData = GameData.Instance;
            OpponentData contractOpponent = gameData.GetOpponent(request.Contract.OpponentId);
            int earnedExperience = gameData.ProgressionConfig.GetBattleExperienceReward(request.IsWin, contractOpponent);
            
            // Валюта
            int earnedCurrency = gameData.RewardConfig.EvaluateMoney(request.IsWin, contractOpponent);
            
            // Предметы
            List<ItemContext> itemRewards = new List<ItemContext> { request.Contract.ItemReward };
            
            DuelRewards result = new(earnedExperience, earnedCurrency, itemRewards );
            
            return result;
        }

        public void ApplyResult(DuelRewards result)
        {
            GameData.Instance.PlayerDataFacade.MetaData.AddExperience(result.ExpDelta);
            
            GameData.Instance.PlayerDataFacade.MetaData.AddMoney(result.CurrencyDelta);
            
            foreach (ItemContext item in result.Items)
                GameData.Instance.PlayerDataFacade.Inventory.AddItem(item);
        }
    }
}