using System.Collections.Generic;
using Blackset.Duel.History;
using Blackset.Duel.Participants;
using Blackset.Duel.Rules;
using Blackset.Duel.TargetValue;
using Blackset.DuelContracts;
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
        public readonly DuelHistory History;

        /// <summary>
        /// Подготовка данных для новой дуэли
        /// </summary>
        /// <param name="contract">Активный контракт</param>
        public DuelContext(DuelContract contract, ActiveStorm storm)
        {
            ServiceGuard.NotNull(contract, nameof(contract));
            
            Rules = new DuelRulesConfiguration();
            Storm = storm;
            Seed = string.Empty;
            
            TargetValue = new TargetValueContext();
            Progress = new DuelProgressContext();
            History = new DuelHistory();
                
            Contract = contract;
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
                newParticipantId = contract.OpponentId;
            }
            else
                newParticipantId = IdGenerator.NewWithPrefix("Player");
            
            DuelParticipantState participantState = new(newParticipantId, isPlayer);
            Participants.Add(newParticipantId, participantState);
    
            return newParticipantId;
        }
        
        #endregion

        public void ApplyActiveStorm()
        {
            if (!Storm.IsActive() || !Storm.TryGet(out Storm activeStorm)) return;
            activeStorm.Apply(ref Rules);
        }
    }
}