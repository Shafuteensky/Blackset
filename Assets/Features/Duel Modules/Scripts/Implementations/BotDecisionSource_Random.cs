using System;
using System.Collections.Generic;
using Blackset.Data.Registries;
using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
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
        
        public SelectionState BuildDeclaration(DuelContext context)
        {
            SelectionState selection = new();
            DuelParticipantState bot = context.Participants[context.OpponentId];
            
            bool passed = Random.value <= PASS_CHANCE;
            if (!passed && 
                TryGetRandomUnused(bot.Sets.DiceSetInventory, bot.FightState.GetUsedDices(), out string unusedDiceId))
            {
                selection.SelectItem(unusedDiceId);
                declaredDice = unusedDiceId;
            }
            else
            {
                selection.SelectItem(string.Empty);
                declaredDice = string.Empty;
            }
            
            return selection;
        }
        
        public SelectionState BuildDiceSelection(DuelContext context)
        {
            SelectionState selection = new();
            DuelParticipantState bot = context.Participants[context.OpponentId];
            
            OpponentData opponent = GameData.Instance.GetOpponent(context.Contract.OpponentId);
            bool isHonest = Random.value < opponent.CunningLevel;
            if (!isHonest && 
                TryGetRandomUnused(bot.Sets.DiceSetInventory, bot.FightState.GetUsedDices(), out string unusedDiceId))
            {
                selection.SelectItem(unusedDiceId);
            }
            else
                selection.SelectItem(declaredDice);
            
            return selection;
        }

        public SelectionState BuildConsumableSelection(DuelContext context)
        {
            SelectionState selection = new();
            DuelParticipantState bot = context.Participants[context.OpponentId];
            
            bool consumableChosen = Random.value <= CONSUMABLE_USE_CHANCE;
            if (consumableChosen && 
                TryGetRandomUnused(bot.Sets.ConsumableSetInventory, bot.FightState.GetUsedConsumables(), out string unusedConsId))
            {
                selection.SelectItem(unusedConsId);
            }
            
            return selection;
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