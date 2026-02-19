using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Effects;
using Extensions.Log;
using UnityEngine;

namespace Features.ItemGenerators
{
    /// <summary>
    /// Базовый генератор предметов
    /// </summary>
    public class ItemsGenerator
    {
        protected readonly DataRegistriesFacade gameData;
        //protected readonly PlayerDataFacade playerData; // TODO данные игрока для определения доступности предметов от стадии прогресса
        
        /// <summary>
        /// Конструктор генератора предметов
        /// </summary>
        /// <param name="gameData">Фасад всех игровых данных</param>
        /// <param name="gameData">Фасад всех данных игрока</param>
        public ItemsGenerator(DataRegistriesFacade gameData)
        {
            if (gameData == null) ServiceDebug.LogError("Ссылки на реестры данных не получены");
            
            this.gameData = gameData;
            //this.playerData = playerData;
        }

        /// <summary>
        /// Получить новый случайный дайс
        /// </summary>
        /// <param name="set">Набор, к которому должен принадлежать дайс (по-умолчанию любой набор)</param>
        /// <returns>Дайс определенного типа и редкости</returns>
        public DiceItemContext GetRandomDice(DiceSet set = null)
        {
            if (gameData == null || gameData.Dices == null || gameData.Dices.Data == null) return new();
            
            DiceItemContext newDice = new();
            
            var allDices = gameData.Dices.Data;
            if (set != null) allDices = GetDicesBySet(set);
            newDice.Item = (DiceItem)allDices[Random.Range(0, allDices.Count)];
            
            var availableDiceTypes = newDice.Item.AvailableTypes;
            newDice.Type = availableDiceTypes[Random.Range(0, availableDiceTypes.Count)] as DiceType;
            
            return newDice;
        }

        /// <summary>
        /// Получить новый случайный расходник
        /// </summary>
        /// <param name="set">Набор, к которому должен принадлежать расходник (по-умолчанию любой набор)</param>
        /// <returns>Расходник определенного типа и редкости</returns>
        public ConsumableItemContext GetRandomConsumable(ConsumableSet set = null)
        {
            if (gameData == null || gameData.Consumables == null || gameData.Consumables.Data == null) return new();
            
            ConsumableItemContext newConsumable = new();
            
            var allConsumables = gameData.Consumables.Data;
            if (set != null) allConsumables = GetConsumablesBySet(set);
            newConsumable.Item = (ConsumableItem)allConsumables[Random.Range(0, allConsumables.Count)];
            
            var availableDiceTypes = newConsumable.Item.AvailableTypes;
            newConsumable.Type = availableDiceTypes[Random.Range(0, availableDiceTypes.Count)] as ConsumableType;
            
            return newConsumable;
        }

        #region Internal
        
        protected List<EffectingItem> GetDicesBySet(DiceSet set)
        {
            if (gameData == null || gameData.Dices == null || gameData.Dices.Data == null) return null;
            
            var allDices = gameData.Dices.Data;
            List<EffectingItem> dicesBySet = new();
            
            foreach (var item in allDices)
            {
                if (item is DiceItem dice && dice.Set == set) dicesBySet.Add(dice);
            }

            return dicesBySet;
        }

        protected List<EffectingItem> GetConsumablesBySet(ConsumableSet set)
        {
            if (gameData == null || gameData.Consumables == null || gameData.Consumables.Data == null) return null;
            
            var allConsumables = gameData.Consumables.Data;
            List<EffectingItem> consumablesBySet = new();
            
            foreach (var item in allConsumables)
            {
                if (item is ConsumableItem consumable && consumable.Set == set) consumablesBySet.Add(consumable);
            }

            return consumablesBySet;
        }
        
        #endregion
    }
}