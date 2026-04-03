using System.Collections.Generic;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.DuelEvents.EventTypes;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Blackset.Opponents;
using Blackset.Player;
using Extensions.FiniteStateMachine;
using Extensions.Log;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 12. Финал дуэли
    /// </summary>
    /// <remarks>
    /// - Закрытие сессии дуэли, возвращение в меню
    /// </remarks>
    public class DuelEndState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            UpdatePlayedOpponentStatistics(context);
            BurnConsumables(context);
            eventHub.Publish(new DuelEndEvent(context.Progress.DuelResult));
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Stop();
        }
        
        public void Exit(DuelContext context) { }

        #region Статистика по сопернику

        /// <summary>
        /// Обновляет статистику побед и поражений против соперника
        /// </summary>
        private void UpdatePlayedOpponentStatistics(DuelContext context)
        {
            PlayerProgressData playerProgress = GameData.Instance.PlayerDataFacade?.ProgressData?.Data;
            if (playerProgress == null)
            {
                ServiceDebug.LogError("Ошибка доступа к данным прогресса игрока, статистика игры по сопернику не учтена");
                return;
            }
            
            DuelParticipantState playerParticipant = null;
            DuelParticipantState opponentParticipant = null;

            foreach (DuelParticipantState participant in context.Participants.Values)
            {
                if (participant.IsPlayer) 
                    playerParticipant = participant;
                else 
                    opponentParticipant = participant;
            }
            if (playerParticipant == null || opponentParticipant == null) return;

            string winnerId = context.Progress.DuelResult.WinnerId;
            if (winnerId == playerParticipant.ParticipantId)
            {
                playerProgress.OpponentDefeat(opponentParticipant.ParticipantId);
                return;
            }
            if (winnerId == opponentParticipant.ParticipantId)
            {
                playerProgress.OpponentWon(opponentParticipant.ParticipantId);
            }
        }

        #endregion

        #region Уничтожение использованных SingleUse расходников

        private void BurnConsumables(DuelContext context)
        {
            foreach (DuelParticipantState participantData in context.Participants.Values)
            {
                Inventory consumableInventory;
                if (participantData.IsPlayer) consumableInventory = GameData.Instance.PlayerDataFacade.ConsumablesPool;
                else consumableInventory = participantData.Sets.ConsumableSetInventory;
                
                List<string> usedConsumables = participantData.FightState.GetUsedConsumables();

                RemoveSingleUseConsumables(participantData, usedConsumables, consumableInventory);
            }
        }

        private void RemoveSingleUseConsumables(DuelParticipantState participantData, List<string> usedConsumables, Inventory consumableInventory)
        {
            foreach (string consumableId in usedConsumables)
            {
                ItemContext consumableInSet = participantData.Sets.ConsumableSetInventory.GetById(consumableId).Item;
                bool containsInPool = consumableInventory.Contains(consumableInSet, out string poolCellId);
                if (!containsInPool) return;
                
                ConsumableType consumableType = (ConsumableType)consumableInventory.GetCellTypeData(poolCellId);
                
                if (consumableType != null && consumableType.SingleUse)
                {
                    consumableInventory.RemoveItem(poolCellId);
                    
                    eventHub.Publish(new ConsumableBurnedEvent(participantData.ParticipantId, consumableId));
                }
            }
        }

        #endregion
    }
}