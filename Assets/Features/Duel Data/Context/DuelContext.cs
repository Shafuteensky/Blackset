using System.Collections.Generic;
using Blackset.Duel.Participants;
using Blackset.Duel.Rules;
using Blackset.Duel.TargetValue;
using Blackset.DuelContracts;
using Blackset.Inventories;
using Blackset.Storms;
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
        /// Правила дуэли
        /// </summary>
        public DuelRulesConfiguration Rules;
        /// <summary>
        /// Текущий шторм
        /// </summary>
        public ActiveStorm Storm { get; private set; }
        /// <summary>
        /// Активный контракт
        /// </summary>
        public DuelContract Contract { get; private set; }
        
        // TODO Состояния активных эффектов
        /// <summary>
        /// Состояния активных эффектов
        /// </summary>
        // public EffectsStateContext effectsState;
        /// <summary>
        /// Сид дуэли
        /// </summary>
        public string Seed;

        /// <summary>
        /// Участники <идентификатор, данные>
        /// </summary>
        public readonly Dictionary<string, DuelParticipantState> Participants = new();
        /// <summary>
        /// Знания об участниках дуэли <идентификатор, знания>
        /// </summary>
        public readonly Dictionary<string, KnowledgeState> Knowledge = new();
        /// <summary>
        /// Идентификатор игрока
        /// </summary>
        public string PlayerId = string.Empty;
        /// <summary>
        /// Идентификатор соперника-бота
        /// </summary>
        public string OpponentId = string.Empty;
        
        /// <summary>
        /// Данные о целевом значении
        /// </summary>
        public TargetValueContext TargetValue = new();
        /// <summary>
        /// Данные о прогрессе дуэли
        /// </summary>
        public DuelProgressContext Progress;

        /// <summary>
        /// Инвентарь для временного хранения сборки дайсов игрока
        /// </summary>
        public readonly Inventory PlayerDiceSetInventory;
        /// <summary>
        /// Инвентарь для временного сборки дайсов бота
        /// </summary>
        public readonly Inventory OpponentDiceSetInventory;
        /// <summary>
        /// Инвентарь для временного хранения сборки расходников игрока
        /// </summary>
        public readonly Inventory PlayerConsumableSetInventory;
        /// <summary>
        /// Инвентарь для временного сборки расходников бота
        /// </summary>
        public readonly Inventory OpponentConsumableSetInventory;

        /// <summary>
        /// Подготовка данных для новой дуэли
        /// </summary>
        /// <param name="contract">Активный контракт</param>
        public DuelContext(DuelContract contract, ActiveStorm storm, 
            Inventory playerDiceSetInventory, Inventory opponentDiceSetInventory,
            Inventory playerConsumableSetInventory, Inventory opponentConsumableSetInventory)
        {
            ServiceGuard.NotNull(contract, nameof(contract));
            ServiceGuard.NotNull(playerDiceSetInventory, nameof(playerDiceSetInventory));
            ServiceGuard.NotNull(opponentDiceSetInventory, nameof(opponentDiceSetInventory));
            ServiceGuard.NotNull(playerConsumableSetInventory, nameof(playerConsumableSetInventory));
            ServiceGuard.NotNull(opponentConsumableSetInventory, nameof(opponentConsumableSetInventory));
            
            Rules = DuelRulesConfiguration.Default();
            Storm = storm;
            Seed = string.Empty;
            
            TargetValue = new TargetValueContext();
            Progress = DuelProgressContext.Default();
                
            Contract = contract;
            
            PlayerDiceSetInventory = playerDiceSetInventory;
            OpponentDiceSetInventory = opponentDiceSetInventory;
            PlayerConsumableSetInventory = playerConsumableSetInventory;
            OpponentConsumableSetInventory = opponentConsumableSetInventory;
        }

        /// <summary>
        /// Применить правила активного шторма
        /// </summary>
        public void ApplyActiveStorm()
        {
            if (!Storm.IsActive() || !Storm.TryGet(out Storm activeStorm)) return;
            activeStorm.Apply(ref Rules);
        }
            
        #region Регистрация участников дуэли
        
        /// <summary>
        /// Регистрация участника: игрок
        /// </summary>
        /// <param name="dices">Дайсы в пуле участника</param>
        /// <param name="consumables">Расходники в пуле участника</param>
        /// <returns>Идентификатор зарегестрированного участника</returns>
        public string RegisterPlayer()
        {
            PlayerId = RegisterParticipant(true);
            return PlayerId;
        }

        /// <summary>
        /// Регистрация участника: оппонент-бот
        /// </summary>
        /// <param name="dices">Дайсы в пуле участника</param>
        /// <param name="consumables">Расходники в пуле участника</param>
        /// <returns>Идентификатор зарегестрированного участника</returns>
        public string RegisterBot(DuelContract contract)
        {
            OpponentId = RegisterParticipant(false, contract);
            return OpponentId;
        }
        
        private string RegisterParticipant(bool isPlayer, DuelContract contract = null)
        {
            string newParticipantId;
            if (!isPlayer)
            {
                ServiceGuard.NotNull(contract, nameof(contract));
                newParticipantId = contract != null ? contract.OpponentId : "";
            }
            else
                newParticipantId = IdGenerator.NewWithPrefix("Player");
            
            DuelParticipantState participantState = new(newParticipantId, isPlayer);
            Participants.Add(newParticipantId, participantState);
    
            return newParticipantId;
        }
        
        #endregion
    }
}