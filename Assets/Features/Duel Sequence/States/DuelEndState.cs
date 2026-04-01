using System.Collections.Generic;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.DuelEvents.EventTypes;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Extensions.FiniteStateMachine;
using UnityEngine;

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
            BurnConsumables(context);
            eventHub.Publish(new DuelEndEvent(context.Progress.DuelResult));
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Stop();
        }
        
        public void Exit(DuelContext context) { }

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