using System;
using System.Collections.Generic;
using Blackset.Data.Registries;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Duel.Targets;
using Blackset.Inventories;
using Blackset.Opponents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Случайные решения бота (Источник построения намерений бота)
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(BotDecisionSource_Random),
        menuName = "Blackset/Duel/Modules/" + nameof(BotDecisionSource_Random))]
    public class BotDecisionSource_Random : BaseDuelModule, IBotDecisionSource
    {
        private const float PASS_CHANCE = 0.05f;
        private const float CONSUMABLE_USE_CHANCE = 0.2f;

        private string declaredDice = String.Empty;
        
        public string BuildDeclaration(DuelContext context)
        {
            declaredDice = String.Empty;
            DuelParticipantState bot = context.Participants[context.OpponentId];

            TryGetRandomUnused(bot.Sets.DiceSetInventory, bot.FightState.DicesUsed, out string unusedDiceId);
            declaredDice = unusedDiceId;
            
            return declaredDice;
        }
        
        public TurnParticipantState BuildIntentState(DuelContext context)
        {
            TurnParticipantState intentState = new TurnParticipantState();
            intentState.ResetForNewTurn();
            DuelParticipantState bot = context.Participants[context.OpponentId];
            
            // Если хитрый - кидает другой дайс (не который объявил)
            OpponentData opponent = GameData.Instance.GetOpponent(context.Contract.OpponentId);
            if (Random.value < opponent.CunningLevel && 
                TryGetRandomUnused(bot.Sets.DiceSetInventory, bot.FightState.DicesUsed, out string unusedDiceId))
            {
                intentState.ChoseDice(unusedDiceId);
            }
            else
                intentState.ChoseDice(declaredDice); 
            
            // Случайный расходник на себя
            bool consumableChosen = Random.value <= CONSUMABLE_USE_CHANCE;
            if (consumableChosen && 
                TryGetRandomUnused(bot.Sets.ConsumableSetInventory, bot.FightState.ConsumablesUsed, out string unusedConsId))
            {
                intentState.ChoseConsumable(unusedConsId, ApplyTarget.Self);
            }
            
            return intentState;
        }

        #region Internal
        
        private bool TryGetRandomUnused(
            Inventory registry,
            List<string> usedKeys,
            out string resultKey)
        {
            resultKey = default;
            if (registry.Data.Count == 0) return false;
            var availableKeys = new List<string>(registry.Data.Count);

            foreach (var inventoryCell in registry.Data)
            {
                if (usedKeys.Contains(inventoryCell.Id) == false) availableKeys.Add(inventoryCell.Id);
            }

            if (availableKeys.Count == 0) return false;

            int randomIndex = Random.Range(0, availableKeys.Count);
            resultKey = availableKeys[randomIndex];

            return true;
        }
        
        #endregion
    }
}