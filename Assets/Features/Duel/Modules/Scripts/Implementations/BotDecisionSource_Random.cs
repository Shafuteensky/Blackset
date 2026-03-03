using System;
using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Registries;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Duel.TurnIntents;
using Blackset.Opponents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Случайные решения бота
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
            OpponentData opponent = GameData.Instance.GetOpponent(context.Contract.OpponentId);
            //opponent.CunningLevel
            
            
            return declaredDice;
        }
        
        public TurnIntent BuildTurnIntent(DuelContext context)
        {
            TurnIntent randomIntents = new TurnIntent();
            DuelParticipantState bot = context.Participants[context.OpponentId];
            
            randomIntents.ChosenDice = TakeRandomUnusedId(context, bot.Sets.DicesSet, bot.FightState.DicesUsed);
            
            randomIntents.ConsumableChosen = Random.value <= CONSUMABLE_USE_CHANCE;
            randomIntents.ChosenConsumable = TakeRandomUnusedId(context, bot.Sets.ConsumablesSet, bot.FightState.ConsumablesUsed);
            randomIntents.ConsumableTarget = (ConsumableTarget)Random.Range(1, Enum.GetValues(typeof(ConsumableTarget)).Length);
            
            return randomIntents;
        }

        #region Inner
        
        private string TakeRandomUnusedId<TValue>(
            DuelContext context,
            Dictionary<string, TValue> registry,
            List<string> used)
        {
            DuelParticipantState bot = context.Participants[context.OpponentId];
            bool hasUnused = TryGetRandomUnused(registry, used, out string randomId);
            if (!hasUnused) return string.Empty;
            return randomId;
        }
        
        private bool TryGetRandomUnused<TKey, TItem>(
            Dictionary<TKey, TItem> registry,
            List<TKey> usedKeys,
            out TKey resultKey)
        {
            resultKey = default;
            if (registry.Count == 0) return false;
            var availableKeys = new List<TKey>(registry.Count);

            foreach (var key in registry.Keys)
            {
                if (usedKeys.Contains(key) == false) availableKeys.Add(key);
            }

            if (availableKeys.Count == 0) return false;

            int randomIndex = UnityEngine.Random.Range(0, availableKeys.Count);
            resultKey = availableKeys[randomIndex];

            return true;
        }
        
        #endregion
    }
}