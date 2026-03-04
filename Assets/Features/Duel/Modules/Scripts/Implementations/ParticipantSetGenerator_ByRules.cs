using System;
using System.Collections.Generic;
using System.Linq;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Duel.Requests;
using Blackset.Duel.Rules;
using Blackset.Duel.Sets;
using Extensions.Log;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Генератор сборок участников дуэли (по правилам дуэли)
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ParticipantSetGenerator_ByRules),
        menuName = "Blackset/Duel/Modules/" + nameof(ParticipantSetGenerator_ByRules))]
    public class ParticipantSetGenerator_ByRules : BaseDuelModule, IParticipantSetGenerator
    {
        public DuelSetsContext GenerateSets(SetGenerationRequest request)
        {
            List<DiceItemContext> diceSet = GenerateDiceSets(request.DuelRules.DiceSetPolicy, 
                request.DuelRules.DicesInSet, request.Pools.DicesPool);
            List<ConsumableItemContext> consumableSet = GenerateConsumableSets(request.DuelRules.ConsumableSetPolicy, 
                request.DuelRules.ConsumablesInSet, request.Pools.ConsumablesPool);
                
            DuelSetsContext setsContext = new DuelSetsContext(diceSet, consumableSet);
            
            return setsContext;
        }

        /// <summary>
        /// Составить сборку дайсов согласно правилам
        /// </summary>
        private List<DiceItemContext> GenerateDiceSets(DiceSetPolicy activeDieSetPolicy, int dicesInSet,
            Dictionary<DiceType, List<DiceItemContext>> dicesPool)
        {
            switch (activeDieSetPolicy)
            {
                case DiceSetPolicy.OneInType:
                    return GetOneRandomDicePerType(dicesPool, dicesInSet);
        
                case DiceSetPolicy.Chaos:
                    return GetRandomDices(dicesPool, dicesInSet);

                default:
                    ServiceDebug.LogError(
                        $"Необработанный случай политики составления сборки дайсов ({activeDieSetPolicy})");
                    return new List<DiceItemContext>();
            }
        }

        /// <summary>
        /// Составить сборку расходников согласно правилам
        /// </summary>
        private List<ConsumableItemContext> GenerateConsumableSets(ConsumableSetPolicy activeConsumableSetPolicy, int consumablesInSet,
            List<ConsumableItemContext> consumablesPool)
        {
            switch (activeConsumableSetPolicy)
            {
                case ConsumableSetPolicy.Random:
                    return GetRandomConsumables(consumablesPool, consumablesInSet);
        
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
        private List<DiceItemContext> GetOneRandomDicePerType(Dictionary<DiceType, List<DiceItemContext>> dicesPool, int limit)
        {
            List<DiceItemContext> diceSet = new();
    
            foreach (var (_, dicesOfType) in dicesPool)
            {
                if (diceSet.Count >= limit)
                    break;
        
                if (dicesOfType.Count == 0)
                    continue;
        
                int randomIndex = Random.Range(0, dicesOfType.Count);
                diceSet.Add(dicesOfType[randomIndex]);
            }
    
            return diceSet;
        }

        /// <summary>
        /// Возвращает ровно <paramref name="limit"/> случайных дайсов из общего пула без учёта типов
        /// </summary>
        private List<DiceItemContext> GetRandomDices(Dictionary<DiceType, List<DiceItemContext>> dicesPool, int limit)
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
                int randomIndex = Random.Range(0, flatPool.Count);
                diceSet.Add(flatPool[randomIndex]);
            }
    
            return diceSet;
        }

        /// <summary>
        /// Возвращает перемешанную копию пула расходников, но не более <paramref name="limit"/> элементов
        /// </summary>
        private List<ConsumableItemContext> GetRandomConsumables(List<ConsumableItemContext> consumablesPool, int limit)
        {
            List<ConsumableItemContext> consumableSet = new(consumablesPool);
    
            for (int i = consumableSet.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (consumableSet[i], consumableSet[j]) = (consumableSet[j], consumableSet[i]);
            }
    
            return consumableSet.Take(limit).ToList();
        }
        
        #endregion
    }
}