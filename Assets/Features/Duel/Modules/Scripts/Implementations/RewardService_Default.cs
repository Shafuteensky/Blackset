using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Registries;
using Blackset.Duel.Requests;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Blackset.Opponents;
using Blackset.Player;
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
            // Опыт (при поражении поощрительный)
            GameData gameData = GameData.Instance;
            OpponentData contractOpponent = gameData.GetOpponent(request.Contract.OpponentId);
            int earnedExperience = gameData.ProgressionConfig.GetBattleExperienceReward(request.IsWin, contractOpponent);
            
            // Валюта (при поражении поощрительный)
            int earnedCurrency = gameData.RewardConfig.EvaluateMoney(request.IsWin, contractOpponent);
            
            // Предметы (только при победе)
            List<ItemContext> itemRewards = new List<ItemContext>();
            if (request.IsWin)
                itemRewards.Add(request.Contract.ItemReward);
            
            DuelRewards result = new(earnedExperience, earnedCurrency, itemRewards );
            
            return result;
        }

        public void ApplyResult(DuelRewards result)
        {
            PlayerDataFacade playerData = GameData.Instance.PlayerDataFacade;
            playerData.MetaData.AddExperience(result.ExpDelta);
            
            playerData.MetaData.AddMoney(result.CurrencyDelta);
            
            foreach (ItemContext item in result.Items)
                playerData.Inventory.AddNewItem(item);
        }
    }
}