using System;
using System.Collections.Generic;
using System.Linq;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Duel.Context;
using Blackset.Duel.Rules;
using Blackset.Duel.Sets;
using Blackset.Inventories;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Генератор сборок участников дуэли (по правилам дуэли)
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ParticipantSetGenerator_ByRules),
        menuName = "Blackset/Duel/Modules/" + nameof(ParticipantSetGenerator_ByRules))]
    public class ParticipantSetGenerator_ByRules : BaseDuelModule, IParticipantSetGenerator
    {
        public DuelSetsContext GenerateSets(DuelContext context, string participantId)
        {
            List<DiceItemContext> dicesInSet = GenerateDiceSets(context, participantId);
            List<ConsumableItemContext> consumablesInSet = GenerateConsumableSets(context, participantId);

            Inventory diceSetInventory;
            Inventory consumableSetInventory;
            if (participantId == context.PlayerId)
            {
                diceSetInventory = context.PlayerDiceSetInventory;
                consumableSetInventory = context.PlayerConsumableSetInventory;
            }
            else
            {
                diceSetInventory = context.OpponentDiceSetInventory;
                consumableSetInventory = context.OpponentConsumableSetInventory;
            }
            
            DuelSetsContext setsContext = new DuelSetsContext(
                diceSetInventory, consumableSetInventory, 
                dicesInSet, consumablesInSet);
            
            return setsContext;
        }

        /// <summary>
        /// Составить сборку дайсов согласно правилам
        /// </summary>
        private List<DiceItemContext> GenerateDiceSets(DuelContext context, string participantId)
        {
            int dicesInSet = context.Rules.DicesInSet;
            DiceSetPolicy activeDieSetPolicy = context.Rules.DiceSetPolicy;
            System.Random random = new(HashCode.Combine(context.Seed, participantId));
            
            Dictionary<DiceType, List<DiceItemContext>> dicesPool = context.Participants[participantId].Pools.DicesPool;
            
            switch (context.Rules.DiceSetPolicy)
            {
                case DiceSetPolicy.OneInType:
                    return GetOneRandomDicePerType(dicesPool, dicesInSet, random);
        
                case DiceSetPolicy.Chaos:
                    return GetRandomDices(dicesPool, dicesInSet, random);
                // TODO AllD20
                // case DiceSetPolicy.AllD20:
                //     var randomDices = GetRandomDices(dicesPool, dicesInSet, random);
                //     foreach (var dice in randomDices)
                //         dice.Type = 
                //     return GetRandomDices(dicesPool, dicesInSet, random);

                default:
                    ServiceDebug.LogError(
                        $"Необработанный случай политики составления сборки дайсов ({activeDieSetPolicy})");
                    return new List<DiceItemContext>();
            }
        }

        /// <summary>
        /// Составить сборку расходников согласно правилам
        /// </summary>
        private List<ConsumableItemContext> GenerateConsumableSets(DuelContext context, string participantId)
        {
            int consumablesInSet = context.Rules.ConsumablesInSet;
            System.Random random = new(HashCode.Combine(context.Seed, participantId));
            
            List<ConsumableItemContext> consumablesPool = context.Participants[participantId].Pools.ConsumablesPool;
            ConsumableSetPolicy activeConsumableSetPolicy = context.Rules.ConsumableSetPolicy;
            
            switch (activeConsumableSetPolicy)
            {
                case ConsumableSetPolicy.RandomLimited:
                    return GetRandomConsumables(consumablesPool, consumablesInSet, random);
        
                case ConsumableSetPolicy.None:
                    return new List<ConsumableItemContext>();

                default:
                    ServiceDebug.LogError(
                        $"Необработанный случай политики составления сборки расходников ({activeConsumableSetPolicy})");
                    return new List<ConsumableItemContext>();
            }
        }

        #region Составление сборок по отдельным правилам
        
        /// <summary>
        /// Возвращает по одному случайному дайсу на каждый тип из пула, но не более <paramref name="limit"/>
        /// </summary>
        private List<DiceItemContext> GetOneRandomDicePerType(Dictionary<DiceType, List<DiceItemContext>> dicesPool, int limit, System.Random random)
        {
            List<DiceItemContext> diceSet = new();
    
            foreach (var (_, dicesOfType) in dicesPool)
            {
                if (diceSet.Count >= limit)
                    break;
        
                if (dicesOfType.Count == 0)
                    continue;
        
                int randomIndex = random.Next(0, dicesOfType.Count);
                diceSet.Add(dicesOfType[randomIndex]);
            }
    
            return diceSet;
        }

        /// <summary>
        /// Возвращает ровно <paramref name="limit"/> случайных дайсов из общего пула без учёта типов
        /// </summary>
        private List<DiceItemContext> GetRandomDices(Dictionary<DiceType, List<DiceItemContext>> dicesPool, int limit, System.Random random)
        {
            List<DiceItemContext> diceSet = new();
    
            List<DiceItemContext> flatPool = dicesPool.Values
                .SelectMany(dices => dices)
                .ToList();
    
            if (flatPool.Count == 0)
                return diceSet;
    
            int slotsCount = Math.Min(limit, flatPool.Count);
    
            for (int i = 0; i < slotsCount; i++)
            {
                int randomIndex = random.Next(0, flatPool.Count);
                diceSet.Add(flatPool[randomIndex]);
                flatPool.RemoveAt(randomIndex);
            }
    
            return diceSet;
        }

        /// <summary>
        /// Возвращает перемешанную копию пула расходников, но не более <paramref name="limit"/> элементов
        /// </summary>
        private List<ConsumableItemContext> GetRandomConsumables(List<ConsumableItemContext> consumablesPool, int limit, System.Random random)
        {
            List<ConsumableItemContext> consumableSet = new(consumablesPool);
    
            for (int i = consumableSet.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (consumableSet[i], consumableSet[j]) = (consumableSet[j], consumableSet[i]);
            }
    
            return consumableSet.Take(limit).ToList();
        }
        
        #endregion
    }
}