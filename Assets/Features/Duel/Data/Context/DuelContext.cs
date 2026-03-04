using System;
using System.Collections.Generic;
using Blacklset.DecisionInput;
using Blackset.Data;
using Blackset.Duel.History;
using Blackset.Duel.Participants;
using Blackset.Duel.Pools;
using Blackset.Duel.Rules;
using Blackset.Duel.TargetValue;
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
        /// 
        /// </summary>
        public DuelInputPresenter InputPresenter;
        
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
        /// Идентификатор игрока
        /// </summary>
        public string PlayerId = String.Empty;
        /// <summary>
        /// Идентификатор соперника-бота
        /// </summary>
        public string OpponentId = String.Empty;
        
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
        public DuelContext(DuelContract contract, DuelInputPresenter input)
        {
            Rules = new DuelRulesConfiguration(); // TODO шторма где применять?
            Seed = IdGenerator.NewGuid();
            
            TargetValue = new TargetValueContext();
            Progress = new DuelProgressContext();
            History = new DuelHistory();
                
            InputPresenter = input;
            
            // Регистрация участников
            Contract = contract;
            //RegisterBot(contract.Opponent.GetDicesPool(), contract.Opponent.GetConsumablesPool(), contract.Opponent);
            // TODO на каком этапе регистрировать игрока?
        }

        #region Регистрация участников дуэли
        
        /// <summary>
        /// Регистрация участника: игрок
        /// </summary>
        /// <param name="dices">Дайсы в пуле участника</param>
        /// <param name="consumables">Расходники в пуле участника</param>
        /// <returns>Идентификатор зарегестрированного участника</returns>
        public string RegisterPlayer(List<DiceItemContext> dices, List<ConsumableItemContext> consumables)
        {
            PlayerId = RegisterParticipant(true, dices, consumables);
            return PlayerId;
        }

        /// <summary>
        /// Регистрация участника: оппонент-бот
        /// </summary>
        /// <param name="dices">Дайсы в пуле участника</param>
        /// <param name="consumables">Расходники в пуле участника</param>
        /// <returns>Идентификатор зарегестрированного участника</returns>
        public string RegisterBot(List<DiceItemContext> dices, List<ConsumableItemContext> consumables, OpponentData botData)
        {
            OpponentId = RegisterParticipant(false, dices, consumables, botData);
            return OpponentId;
        }
        
        private string RegisterParticipant(bool isPlayer, List<DiceItemContext> dices, List<ConsumableItemContext> consumables, OpponentData botData = null)
        {
            ServiceGuard.NotNull(dices, nameof(dices));
            ServiceGuard.NotNull(consumables, nameof(consumables));
            ServiceGuard.IsTrue(dices.Count > 0, "Список дайсов не должен быть пустым");
            ServiceGuard.IsTrue(consumables.Count > 0, "Список расходников не должен быть пустым");

            string newParticipantId;
            if (!isPlayer)
            {
                ServiceGuard.NotNull(botData, nameof(botData));
                newParticipantId = botData.Id;
            }
            else
                newParticipantId = IdGenerator.NewWithPrefix("Player");
            
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