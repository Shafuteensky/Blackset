using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;
using UnityEngine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 7. Броски дайсов (повторяемое состояние)
    /// </summary>
    /// <remarks>
    /// - Генерация результатов бросков и запись
    /// </remarks>
    public class RollResolveState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            IDiceRollPipeline diceRoller = modules.Get<IDiceRollPipeline>();
            IDuelScoreResolver duelScoreResolver = modules.Get<IDuelScoreResolver>();
            
            // Броски дайсов
            foreach (string participantId in context.Participants.Keys)
            {
                DuelParticipantState participant = context.Participants[participantId];
                FightParticipantState participantFightState = participant.FightState;
                if (participantFightState.TurnState.HasPassed.Value) continue;
                
                string chosenDiceId = participantFightState.TurnState.ChosenDice.Value;
                int rollResult = diceRoller.RollDice(context, participantId, chosenDiceId, out bool isCrit);
                
                participantFightState.RegisterRawRollResult(chosenDiceId, rollResult); 

                // Зачет очков дуэли за криты дайсов
                duelScoreResolver.ResolveCrit(participant, isCrit);
                
                eventHub.Publish(new DiceRolledEvent(participantId, chosenDiceId, rollResult));
            }
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<ScoreCommitState>();
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}