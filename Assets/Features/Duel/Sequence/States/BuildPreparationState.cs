using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Registries;
using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.Duel.Pools;
using Blackset.Duel.Requests;
using Blackset.DuelEvents.EventTypes;
using Blackset.Inventories;
using Blackset.Inventories.Helpers;
using Blackset.Opponents;
using Blackset.Player;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 2. Подготовка билдов игроков
    /// </summary>
    /// <remarks>
    /// - Сборка пулов участников
    /// - Применение правил, связанных с пулами
    /// </remarks>
    public class BuildPreparationState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            foreach (DuelParticipantState participant in context.Participants.Values)
            {
                List<DiceItemContext> dices = new();
                List<ConsumableItemContext> consumables = new();
                
                // Пулы из инвентарей игрока
                if (participant.IsPlayer)
                {
                    PlayerDataFacade playerData = GameData.Instance.PlayerDataFacade;
                    
                    foreach (Inventory dicePool in playerData.DicesPoolRows)
                    {
                        foreach (DiceItemContext dice in ItemConverter.ToDiceItemContext(dicePool))
                        {
                            dices.Add(dice);
                        }
                    }
                    consumables = ItemConverter.ToConsumableItemContext(playerData.ConsumablesPool);
                }
                // Пулы из данных ботов
                else
                {
                    OpponentData botData = GameData.Instance.Opponents.GetById(participant.ParticipantId);
                    dices = botData.GetDicesPool();
                    consumables = botData.GetConsumablesPool();
                }
                
                PoolBuildRequest buildRequest = new PoolBuildRequest(context.Rules, dices, consumables);
                
                IParticipantPoolBuilder poolBuilder = modules.Get<IParticipantPoolBuilder>();
                DuelPoolsContext newPools = poolBuilder.BuildPools(buildRequest);
                participant.InitializePools(newPools);
            }
            
            eventHub.Publish(new BuildPreparedEvent());
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<BuildResolveState>();
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}