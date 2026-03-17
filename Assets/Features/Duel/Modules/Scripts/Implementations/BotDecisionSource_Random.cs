using System;
using System.Collections.Generic;
using System.Linq;
using Blackset.Data.Registries;
using Blackset.DecisionInput;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
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
        
        public SelectionState BuildDiceSelection(DuelContext context)
        {
            SelectionState selection = new();
            DuelParticipantState bot = context.Participants[context.OpponentId];
            
            // Если хитрый - кидает другой дайс (не который объявил)
            OpponentData opponent = GameData.Instance.GetOpponent(context.Contract.OpponentId);
            if (Random.value < opponent.CunningLevel && 
                TryGetRandomUnused(bot.Sets.DiceSetInventory, bot.FightState.DicesUsed, out string unusedDiceId))
            {
                selection.SelectItem(unusedDiceId);
            }
            else
                selection.SelectItem(declaredDice);
            
            SelectRandomParticipantTarget(context, selection);
            SelectRandomDiceTarget(context, selection);
            
            return selection;
        }

        public SelectionState BuildConsumableSelection(DuelContext context)
        {
            SelectionState selection = new();
            DuelParticipantState bot = context.Participants[context.OpponentId];
            
            // Случайный расходник на случайную цель
            bool consumableChosen = Random.value <= CONSUMABLE_USE_CHANCE;
            if (consumableChosen && 
                TryGetRandomUnused(bot.Sets.ConsumableSetInventory, bot.FightState.ConsumablesUsed, out string unusedConsId))
            {
                selection.SelectItem(unusedConsId);
                SelectRandomParticipantTarget(context, selection);
                SelectRandomDiceTarget(context, selection);
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

        private void SelectRandomParticipantTarget(DuelContext context, SelectionState selection)
        {
            string randomParticipant = context.Participants.ElementAt(Random.Range(0, context.Participants.Count)).Key;
            
            selection.SelectTargetParticipant(randomParticipant);
        }

        private void SelectRandomDiceTarget(DuelContext context, SelectionState selection)
        {
            string randomParticipant = context.Participants.ElementAt(Random.Range(0, context.Participants.Count)).Key;
            List<InventoryCell> participantSet = context.Participants[randomParticipant].Sets.DiceSetInventory.Data;
            int randomDice = Random.Range(0, participantSet.Count);
            string randomTargetDice = participantSet[randomDice].Id;
            
            selection.SelectTargetDice(randomTargetDice);
        }
        
        #endregion
    }
}