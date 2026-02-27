using System;
using System.Collections.Generic;
using Blackset.Data;
using Blackset.Duel.History;
using Blackset.Duel.Participants;
using Blackset.Duel.Rules;
using Blackset.DuelContracts;
using Blackset.Opponents;
using Extensions.Helpers;
using Extensions.Log;

namespace Blackset.Duel.Context
{
    /// <summary>
    /// Данные дуэли
    /// </summary>
    public class DuelContext
    {
        /// <summary>
        /// Состояния активных эффектов
        /// </summary>
        // public EffectsStateContext effectsState;
        /// <summary>
        /// Правила дуэли
        /// </summary>
        public DuelRulesConfiguration Rules;
        /// <summary>
        /// Сид дуэли
        /// </summary>
        public string Seed;
        /// <summary>
        /// Активный контракт
        /// </summary>
        public DuelContract Contract;

        /// <summary>
        /// Участники <идентификатор, данные>
        /// </summary>
        public Dictionary<string, DuelParticipantState> Participants = new();
        /// <summary>
        /// Знания об участниках дуэли <идентификатор, знания>
        /// </summary>
        public Dictionary<string, KnowledgeState> PlayerKnowledge = new();
        
        /// <summary>
        /// Денные о целевом значении
        /// </summary>
        public TargetValueContext TargetValue = new();
        /// <summary>
        /// Данные о прогрессе дуэли
        /// </summary>
        public DuelProgressContext Progress;
        
        /// <summary>
        /// История ходов
        /// </summary>
        public DuelHistory History;

        /// <summary>
        /// Подготовка данных для новой дуэли
        /// </summary>
        /// <param name="contract">Активный контракт</param>
        public DuelContext(DuelContract contract)
        {
            Rules = new DuelRulesConfiguration(); // TODO шторма где применять?
            Seed = IdGenerator.NewGuid();
            
            TargetValue = new TargetValueContext();
            Progress = new DuelProgressContext();
            History = new DuelHistory();
                
            // Регистрация участников
            Contract = contract;
            //RegisterBot(contract.Opponent.GetDicesPool(), contract.Opponent.GetConsumablesPool(), contract.Opponent);
            // TODO на каком этапе регистрировать игрока?
        }

        #region Регистрация участников дуэли
        
        private string RegisterPlayer(List<DiceItemContext> dices, List<ConsumableItemContext> consumables)
        {
            if (dices == null || consumables == null || dices.Count == 0 || consumables.Count == 0)
            {
                ServiceDebug.LogError($"Получены неполные исходные данные о пулах, игрок не зарегестрирован");
                return String.Empty;
            }
            return RegisterParticipant(true, dices, consumables);
        }

        private string RegisterBot(List<DiceItemContext> dices, List<ConsumableItemContext> consumables, OpponentData botData)
        {
            if (dices == null || consumables == null || dices.Count == 0 || consumables.Count == 0 || botData == null)
            {
                ServiceDebug.LogError($"Получены неполные исходные данные о пулах или сопернике, бот не зарегестрирован");
                return String.Empty;
            }
            return RegisterParticipant(false, dices, consumables, botData);
        }
        
        private string RegisterParticipant(bool isPlayer, List<DiceItemContext> dices, List<ConsumableItemContext> consumables, OpponentData botData = null)
        {
            if (!isPlayer && botData == null)
            {
                ServiceDebug.LogError($"Данные {nameof(OpponentData)} бота отсутствуют, участник не зарегестрирован");
                return String.Empty;
            }
            
            string newParticipantId = IdGenerator.NewWithPrefix(isPlayer ? "Player" : "AI");
            DuelPoolsContext itemPools = new DuelPoolsContext(dices, consumables);
            DuelParticipantState participantState = new(newParticipantId, isPlayer, itemPools);
            KnowledgeState participantKnowledgeState = new();
            
            Participants.Add(newParticipantId, participantState);
            PlayerKnowledge.Add(newParticipantId, participantKnowledgeState);
            
            return newParticipantId;
        }
        
        #endregion
    }
}