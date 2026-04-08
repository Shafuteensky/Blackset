using System;
using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;
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
    // ИИ-код TODO написать нового с учетом решения по эффектам, штормам и знаниям
    /// <summary>
    /// Умный бот: выбирает куб по вероятности достижения целевого значения
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(BotDecisionSource_DiceSidesBased),
        menuName = "Blackset/Duel/Modules/" + nameof(BotDecisionSource_DiceSidesBased))]
    public class BotDecisionSource_DiceSidesBased : BaseDuelModule, IBotDecisionSource
    {
        private const float BASE_CONSUMABLE_USE_CHANCE = 0.15f;
        private const float CONSUMABLE_SELF_TARGET_CHANCE = 0.5f;

        [Header("Оценка выбора"), Space]
        [SerializeField]
        protected float exactChanceWeight = 6f;
        [SerializeField]
        protected float bestSafeGainWeight = 3.5f;
        [SerializeField]
        protected float averageSafeGainWeight = 2f;
        [SerializeField]
        protected float safeChanceWeight = 0.75f;
        [SerializeField]
        protected float overshootPenaltyWeight = 1.5f;

        [Header("Поведение"), Space]
        [SerializeField]
        protected float minPassThreshold = 0.05f;
        [SerializeField]
        protected float maxPassThreshold = 0.35f;
        [SerializeField]
        protected float randomSelectionScoreJitter = 0.25f;

        protected string declaredDice = String.Empty;

        public SelectionState BuildDeclaration(DuelContext context)
        {
            SelectionState selection = new();
            DuelParticipantState bot = context.Participants[context.OpponentId];
            OpponentData opponent = GameData.Instance.GetOpponent(context.Contract.OpponentId);

            List<DiceCandidate> candidates = GetDiceCandidates(context, bot);
            string selectedDiceId = SelectDiceBySkill(candidates, opponent);

            if (string.IsNullOrEmpty(selectedDiceId))
            {
                selection.SelectItem(string.Empty);
                declaredDice = string.Empty;
                return selection;
            }

            bool shouldLie = ShouldLie(opponent);
            if (shouldLie && TryGetAlternativeDeclaration(candidates, selectedDiceId, out string alternativeDiceId))
            {
                selection.SelectItem(alternativeDiceId);
                declaredDice = alternativeDiceId;
                return selection;
            }

            selection.SelectItem(selectedDiceId);
            declaredDice = selectedDiceId;

            return selection;
        }

        public SelectionState BuildDiceSelection(DuelContext context)
        {
            SelectionState selection = new();
            DuelParticipantState bot = context.Participants[context.OpponentId];
            OpponentData opponent = GameData.Instance.GetOpponent(context.Contract.OpponentId);

            List<DiceCandidate> candidates = GetDiceCandidates(context, bot);
            string selectedDiceId = SelectDiceBySkill(candidates, opponent);

            if (string.IsNullOrEmpty(selectedDiceId))
            {
                selection.SelectItem(string.Empty);
                return selection;
            }

            selection.SelectItem(selectedDiceId);
            return selection;
        }

        public SelectionState BuildConsumableSelection(DuelContext context)
        {
            SelectionState selection = new();
            DuelParticipantState bot = context.Participants[context.OpponentId];
            TurnParticipantState turnState = bot.FightState.TurnState;
            OpponentData opponent = GameData.Instance.GetOpponent(context.Contract.OpponentId);

            float consumableChance =
                BASE_CONSUMABLE_USE_CHANCE +
                opponent.MasteryLevel * 0.15f +
                opponent.DifficultyLevel() * 0.15f;

            bool consumableChosen = Random.value <= Mathf.Clamp01(consumableChance);

            if (consumableChosen &&
                TryGetRandomUnused(bot.Sets.ConsumableSetInventory, bot.FightState.GetUsedConsumables(), out string unusedConsId))
            {
                selection.SelectItem(unusedConsId);
                turnState.SetSelectedTargetParticipantId(GetRandomTargetParticipantId(context));
            }
            else
            {
                turnState.SetSelectedTargetParticipantId(context.OpponentId);
            }

            return selection;
        }

        #region Core Logic

        protected List<DiceCandidate> GetDiceCandidates(DuelContext context, DuelParticipantState bot)
        {
            List<DiceCandidate> candidates = new();

            int currentScore = bot.FightState.FightScore.Value;
            int targetScore = context.TargetValue.TargetValue.Value;
            int needed = targetScore - currentScore;

            if (needed <= 0)
            {
                return candidates;
            }

            List<string> used = bot.FightState.GetUsedDices();
            Inventory inventory = bot.Sets.DiceSetInventory;

            foreach (InventoryCell cell in inventory.Data)
            {
                if (used.Contains(cell.Id))
                {
                    continue;
                }

                int[] sideNumbers = GetDiceSideNumbers(cell.Item);
                if (sideNumbers == null || sideNumbers.Length == 0)
                {
                    continue;
                }

                float score = EvaluateDice(sideNumbers, needed);

                candidates.Add(new DiceCandidate
                {
                    DiceId = cell.Id,
                    Score = score,
                    SideNumbers = sideNumbers
                });
            }

            candidates.Sort((left, right) => right.Score.CompareTo(left.Score));

            return candidates;
        }

        protected string SelectDiceBySkill(List<DiceCandidate> candidates, OpponentData opponent)
        {
            if (candidates == null || candidates.Count == 0)
            {
                return String.Empty;
            }

            float mastery = opponent.MasteryLevel;
            float difficulty = opponent.DifficultyLevel();
            float passThreshold = Mathf.Lerp(maxPassThreshold, minPassThreshold, difficulty);

            DiceCandidate bestCandidate = candidates[0];
            if (bestCandidate.Score < passThreshold)
            {
                return String.Empty;
            }

            if (mastery <= 0.05f)
            {
                int randomIndex = Random.Range(0, candidates.Count);
                DiceCandidate randomCandidate = candidates[randomIndex];

                if (randomCandidate.Score < passThreshold)
                {
                    return String.Empty;
                }

                return randomCandidate.DiceId;
            }

            if (mastery >= 0.95f)
            {
                return bestCandidate.DiceId;
            }

            List<WeightedDiceCandidate> weightedCandidates = BuildWeightedCandidates(candidates, mastery, passThreshold);
            if (weightedCandidates.Count == 0)
            {
                return String.Empty;
            }

            return ChooseWeightedCandidate(weightedCandidates);
        }

        protected List<WeightedDiceCandidate> BuildWeightedCandidates(
            List<DiceCandidate> candidates,
            float mastery,
            float passThreshold)
        {
            List<WeightedDiceCandidate> result = new();

            float bestScore = candidates[0].Score;
            float worstScore = candidates[^1].Score;
            float scoreRange = Mathf.Max(0.001f, bestScore - worstScore);

            foreach (var candidate in candidates)
            {
                if (candidate.Score < passThreshold)
                {
                    continue;
                }

                float normalizedQuality = (candidate.Score - worstScore) / scoreRange;
                float masteryBias = Mathf.Lerp(0.25f, 4f, mastery);
                float noisyQuality = normalizedQuality + Random.Range(-randomSelectionScoreJitter, randomSelectionScoreJitter) * (1f - mastery);
                float weight = Mathf.Pow(Mathf.Clamp01(noisyQuality), masteryBias);

                if (weight <= 0f)
                {
                    continue;
                }

                result.Add(new WeightedDiceCandidate
                {
                    DiceId = candidate.DiceId,
                    Weight = weight
                });
            }

            return result;
        }

        protected string ChooseWeightedCandidate(List<WeightedDiceCandidate> candidates)
        {
            float totalWeight = 0f;

            foreach (var t in candidates)
            {
                totalWeight += t.Weight;
            }

            if (totalWeight <= 0f)
            {
                return String.Empty;
            }

            float randomValue = Random.value * totalWeight;
            float currentWeight = 0f;

            foreach (var t in candidates)
            {
                currentWeight += t.Weight;

                if (randomValue <= currentWeight)
                {
                    return t.DiceId;
                }
            }

            return candidates[^1].DiceId;
        }

        protected bool ShouldLie(OpponentData opponent)
        {
            return Random.value < opponent.CunningLevel;
        }

        protected bool TryGetAlternativeDeclaration(
            List<DiceCandidate> candidates,
            string actualDiceId,
            out string alternativeDiceId)
        {
            alternativeDiceId = String.Empty;

            if (candidates == null || candidates.Count <= 1)
            {
                return false;
            }

            List<DiceCandidate> alternatives = new();

            foreach (var t in candidates)
            {
                if (t.DiceId == actualDiceId)
                {
                    continue;
                }

                alternatives.Add(t);
            }

            if (alternatives.Count == 0)
            {
                return false;
            }

            int randomIndex = Random.Range(0, alternatives.Count);
            alternativeDiceId = alternatives[randomIndex].DiceId;

            return true;
        }

        protected float EvaluateDice(int[] sideNumbers, int needed)
        {
            if (sideNumbers == null || sideNumbers.Length == 0)
            {
                return float.MinValue;
            }

            if (needed <= 0)
            {
                return float.MinValue;
            }

            int exactCount = 0;
            int safeCount = 0;
            int bestSafeValue = Int32.MinValue;
            float safeSum = 0f;

            foreach (var value in sideNumbers)
            {
                if (value == needed)
                {
                    exactCount++;
                }

                if (value <= needed)
                {
                    safeCount++;
                    safeSum += value;

                    if (value > bestSafeValue)
                    {
                        bestSafeValue = value;
                    }
                }
            }

            float exactChance = (float)exactCount / sideNumbers.Length;
            float safeChance = (float)safeCount / sideNumbers.Length;
            float overshootChance = 1f - safeChance;

            float averageSafeGain = safeCount > 0 ? safeSum / safeCount : 0f;
            float normalizedAverageSafeGain = averageSafeGain / needed;
            float normalizedBestSafeGain = bestSafeValue > 0 ? (float)bestSafeValue / needed : 0f;

            float score =
                exactChance * exactChanceWeight +
                normalizedBestSafeGain * bestSafeGainWeight +
                normalizedAverageSafeGain * averageSafeGainWeight +
                safeChance * safeChanceWeight -
                overshootChance * overshootPenaltyWeight;

            return score;
        }

        protected int[] GetDiceSideNumbers(ItemContext diceItem)
        {
            DiceData itemData = (DiceData)diceItem.GetItemData();
            DiceType typeData = (DiceType)diceItem.GetTypeData();

            return itemData.NumbersConfig.GetSideNumbers(typeData);
        }

        #endregion

        #region Utils

        protected string GetRandomTargetParticipantId(DuelContext context)
        {
            return Random.value < CONSUMABLE_SELF_TARGET_CHANCE
                ? context.OpponentId
                : context.PlayerId;
        }

        protected bool TryGetRandomUnused(
            Inventory registry,
            List<string> usedKeys,
            out string resultKey)
        {
            resultKey = String.Empty;

            if (registry.Data.Count == 0)
            {
                return false;
            }

            List<string> availableKeys = new(registry.Data.Count);

            foreach (InventoryCell cell in registry.Data)
            {
                if (!usedKeys.Contains(cell.Id))
                {
                    availableKeys.Add(cell.Id);
                }
            }

            if (availableKeys.Count == 0)
            {
                return false;
            }

            resultKey = availableKeys[Random.Range(0, availableKeys.Count)];
            return true;
        }

        #endregion

        #region Nested Types

        protected sealed class DiceCandidate
        {
            public string DiceId;
            public float Score;
            public int[] SideNumbers;
        }

        protected sealed class WeightedDiceCandidate
        {
            public string DiceId;
            public float Weight;
        }

        #endregion
    }
}