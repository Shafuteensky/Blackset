using Blackset.ItemGenerators;
using Blackset.Data.Registries;
using Blackset.Duel.Rules;
using Blackset.Inventories.Cells;
using Blackset.Inventories.Scripts.Items;
using Blackset.ItemsRestrictions;
using Blackset.Opponents;
using Extensions.Data.InMemoryData;
using Extensions.Log;
using UnityEngine;

namespace Blackset.DuelContracts
{
    /// <summary>
    /// Запись о сопернике в пуле доступных контрактов
    /// </summary>
    public class DuelContract : InMemoryDataEntry
    {
        /// <summary>
        /// Идентификатор данных оппонента
        /// </summary>
        public string OpponentId { get; private set; }
        
        /// <summary>
        /// Награда в валюте
        /// </summary>
        public int MoneyReward => GameData.Instance.RewardConfig.EvaluateMoney(true, GameData.Instance.GetOpponent(OpponentId));
        
        /// <summary>
        /// Наградной предмет
        /// </summary>
        public ItemContext ItemReward { get; private set; }
        
        /// <summary>
        /// Режим дуэли
        /// </summary>
        public DuelMode Mode { get; private set; }
        
        /// <summary>
        /// Новый контракт
        /// </summary>
        /// <param name="opponent">Данные соперника</param>
        public DuelContract(OpponentData opponent, DuelMode mode = DuelMode.Standard) 
        {
            if (opponent == null)
            {
                ServiceDebug.LogError("Невалидные данные оппонента при создании контракта");
                return;
            }
            
            OpponentId = opponent.Id;
            Mode = mode;

            ItemsGenerator itemsGenerator = new ItemsGenerator();
            ItemClass rewardItemClass = GetRandomRewardItemClass();

            ItemReward = itemsGenerator.GetRandomItem(rewardItemClass, ItemAvailability.Reward);

            if (string.IsNullOrEmpty(ItemReward.ItemId))
            {
                ItemClass fallbackItemClass = rewardItemClass == ItemClass.Dice
                    ? ItemClass.Consumable
                    : ItemClass.Dice;

                ItemReward = itemsGenerator.GetRandomItem(fallbackItemClass, ItemAvailability.Reward);

                if (string.IsNullOrEmpty(ItemReward.ItemId))
                {
                    ServiceDebug.LogError("Не удалось подобрать наградной предмет для контракта");
                }
            }
        }
        
        /// <summary>
        /// Пустой контракт (для сериализации json)
        /// </summary>
        public DuelContract() { }

        /// <summary>
        /// Получить случайный класс наградного предмета
        /// </summary>
        protected ItemClass GetRandomRewardItemClass()
        {
            return Random.value < 0.5f
                ? ItemClass.Dice
                : ItemClass.Consumable;
        }
    }
}