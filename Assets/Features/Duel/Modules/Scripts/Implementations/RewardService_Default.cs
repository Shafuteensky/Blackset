using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Registries;
using Blackset.Duel.Requests;
using Blackset.Opponents;
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
            // Опыт
            GameData gameData = GameData.Instance;
            OpponentData contractOpponent = gameData.GetOpponent(request.Contract.OpponentId);
            int earnedExperience = gameData.ProgressionConfig.GetBattleExperienceReward(request.IsWin, contractOpponent);
            
            // Валюта
            int earnedCurrency = gameData.RewardConfig.EvaluateMoney(request.IsWin, contractOpponent);
            
            // Предметы
            // TODO Генерация нагрдного предмета? Случайный набор? Заранее подготовленная награда контракта?
            List<DiceItemContext> earnedDices = new List<DiceItemContext>(); 
            List<ConsumableItemContext> earnedConsumables = new List<ConsumableItemContext>();
            
            DuelRewards result = new(earnedExperience, earnedCurrency, earnedDices, earnedConsumables);
            
            return result;
        }

        public void ApplyResult(DuelRewards result)
        {
            GameData.Instance.PlayerDataFacade.MetaData.AddExperience(result.ExpDelta);
            
            GameData.Instance.PlayerDataFacade.MetaData.AddMoney(result.CurrencyDelta);
            
            foreach (DiceItemContext dice in result.Dices)
                GameData.Instance.PlayerDataFacade.DicesInventory.AddItem(dice.Dice.Id, dice.Type.Id);

            foreach (ConsumableItemContext consumable in result.Consumables)
                GameData.Instance.PlayerDataFacade.ConsumablesInventory.AddItem(consumable.Data.Id, consumable.Type.Id);
        }
    }
}