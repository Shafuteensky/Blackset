using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Registries;
using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.Inventories.Cells;
using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Стандартный сборщик источников эффектов
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(EffectSourceCollector_Default),
        menuName = "Blackset/Duel/Modules/" + nameof(EffectSourceCollector_Default))]
    public class EffectSourceCollector_Default : BaseDuelModule, IEffectSourceCollector
    {
        /// <summary>
        /// Собрать источники эффектов участника для указанной фазы
        /// </summary>
        /// <param name="context">Контекст дуэли</param>
        /// <param name="participantId">Идентификатор участника</param>
        /// <param name="phase">Текущая фаза применения эффектов</param>
        /// <returns>Список источников эффектов</returns>
        public List<EffectSourceRef> Collect(DuelContext context, string participantId, EffectPhase phase)
        {
            List<EffectSourceRef> sources = new List<EffectSourceRef>();

            if (!context.Participants.TryGetValue(participantId, out DuelParticipantState participant))
                return sources;

            TurnParticipantState turnState = participant.FightState.TurnState;

            CollectSelectedDiceSource(participant, turnState, phase, sources);
            CollectSelectedConsumableSource(participant, turnState, phase, sources);
            CollectUsedDiceSources(participant, phase, sources);
            CollectUsedConsumableSources(participant, phase, sources);

            return sources;
        }
        
        #region Сборка данных
        
        /// <summary>
        /// Собрать источник эффекта выбранного дайса
        /// </summary>
        private void CollectSelectedDiceSource(
            DuelParticipantState participant,
            TurnParticipantState turnState,
            EffectPhase phase,
            List<EffectSourceRef> sources)
        {
            if (!turnState.IsDiceChosen.Value) return;

            string diceInstanceId = turnState.SelectedDice.Value;
            if (string.IsNullOrEmpty(diceInstanceId)) return;

            InventoryCell diceCell = participant.Sets.DiceSetInventory.GetById(diceInstanceId);
            if (diceCell == null) return;

            GameData gameData = GameData.Instance;
            DiceData diceData = gameData.GetDice(diceCell.Item.ItemId);
            if (diceData == null || diceData.Effect == null) return;

            if (diceData.Effect.GetEffectPhase() != phase) return;

            participant.FightState.GetOrCreateDiceUsageState(diceInstanceId);

            sources.Add(new EffectSourceRef(
                diceInstanceId,
                EffectSourceKind.Dice,
                diceData.Effect));
        }

        /// <summary>
        /// Собрать источник эффекта выбранного расходника
        /// </summary>
        private void CollectSelectedConsumableSource(
            DuelParticipantState participant,
            TurnParticipantState turnState,
            EffectPhase phase,
            List<EffectSourceRef> sources)
        {
            if (!turnState.IsConsumableChosen.Value) return;

            string consumableInstanceId = turnState.SelectedConsumable.Value;
            if (string.IsNullOrEmpty(consumableInstanceId)) return;

            InventoryCell consumableCell = participant.Sets.ConsumableSetInventory.GetById(consumableInstanceId);
            if (consumableCell == null) return;

            GameData gameData = GameData.Instance;
            ConsumableData consumableData = gameData.GetConsumable(consumableCell.Item.ItemId);
            if (consumableData == null || consumableData.Effect == null) return;

            if (consumableData.Effect.GetEffectPhase() != phase) return;

            participant.FightState.GetOrCreateConsumableUsageState(consumableInstanceId);

            sources.Add(new EffectSourceRef(
                consumableInstanceId,
                EffectSourceKind.Consumable,
                consumableData.Effect));
        }

        /// <summary>
        /// Собрать уже использованные дайсы, которые могут сработать в текущую фазу
        /// </summary>
        private void CollectUsedDiceSources(
            DuelParticipantState participant,
            EffectPhase phase,
            List<EffectSourceRef> sources)
        {
            List<string> usedDices = participant.FightState.GetUsedDices();

            foreach (string diceInstanceId in usedDices)
            {
                if (string.IsNullOrEmpty(diceInstanceId)) continue;

                InventoryCell diceCell = participant.Sets.DiceSetInventory.GetById(diceInstanceId);
                if (diceCell == null) continue;

                GameData gameData = GameData.Instance;
                DiceData diceData = gameData.GetDice(diceCell.Item.ItemId);
                if (diceData == null || diceData.Effect == null) continue;

                if (diceData.Effect.GetApplyPolicy() == EffectApplyPolicy.OnUse) continue;
                if (diceData.Effect.GetEffectPhase() != phase) continue;

                ItemUsageState usageState = participant.FightState.GetOrCreateDiceUsageState(diceInstanceId);
                if (!usageState.IsUsed || usageState.IsConsumed) continue;

                if (ContainsSource(sources, diceInstanceId, EffectSourceKind.Dice)) continue;

                sources.Add(new EffectSourceRef(
                    diceInstanceId,
                    EffectSourceKind.Dice,
                    diceData.Effect));
            }
        }

        /// <summary>
        /// Собрать уже использованные расходники, которые могут сработать в текущую фазу
        /// </summary>
        private void CollectUsedConsumableSources(
            DuelParticipantState participant,
            EffectPhase phase,
            List<EffectSourceRef> sources)
        {
            List<string> usedConsumables = participant.FightState.GetUsedConsumables();

            foreach (string consumableInstanceId in usedConsumables)
            {
                if (string.IsNullOrEmpty(consumableInstanceId)) continue;

                InventoryCell consumableCell = participant.Sets.ConsumableSetInventory.GetById(consumableInstanceId);
                if (consumableCell == null) continue;

                GameData gameData = GameData.Instance;
                ConsumableData consumableData = gameData.GetConsumable(consumableCell.Item.ItemId);
                if (consumableData == null || consumableData.Effect == null) continue;
                
                if (consumableData.Effect.GetApplyPolicy() == EffectApplyPolicy.OnUse) continue;
                if (consumableData.Effect.GetEffectPhase() != phase) continue;

                ItemUsageState usageState = participant.FightState.GetOrCreateConsumableUsageState(consumableInstanceId);
                if (!usageState.IsUsed || usageState.IsConsumed) continue;

                if (ContainsSource(sources, consumableInstanceId, EffectSourceKind.Consumable)) continue;

                sources.Add(new EffectSourceRef(
                    consumableInstanceId,
                    EffectSourceKind.Consumable,
                    consumableData.Effect));
            }
        }
        
        #endregion

        #region Хелперы
        
        /// <summary>
        /// Проверить, содержится ли уже источник в списке
        /// </summary>
        private bool ContainsSource(List<EffectSourceRef> sources, string sourceInstanceId, EffectSourceKind sourceKind)
        {
            foreach (EffectSourceRef source in sources)
                if (source.SourceInstanceId == sourceInstanceId && source.SourceKind == sourceKind) return true;

            return false;
        }
        
        #endregion
    }
}