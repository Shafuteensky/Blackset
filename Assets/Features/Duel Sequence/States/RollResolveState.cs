using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 7. Броски дайсов, трата расходников (повторяемое состояние)
    /// </summary>
    /// <remarks>
    /// - Генерация результатов бросков и запись в рабочий снапшот текущего броска
    /// </remarks>
    public class RollResolveState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            IDiceRollPipeline diceRoller = modules.Get<IDiceRollPipeline>();
            IDuelScoreResolver duelScoreResolver = modules.Get<IDuelScoreResolver>();
            
            foreach (string participantId in context.Participants.Keys)
            {
                DuelParticipantState participant = context.Participants[participantId];
                FightParticipantState participantFightState = participant.FightState;
                
                if (participantFightState.TurnState.HasPassed.Value) continue;

                if (participantFightState.TurnState.IsDiceChosen.Value)
                {
                    string chosenDiceId = participantFightState.TurnState.SelectedDice.Value;
                    int rawResult = diceRoller.RollDice(context, participantId, chosenDiceId, out bool isCrit);
                    participantFightState.RegisterRawRollResult(chosenDiceId, rawResult);
                    participantFightState.MarkDiceUsed(chosenDiceId, participantId);
                    
                    duelScoreResolver.ResolveCrit(context, participantId, isCrit);
                    eventHub.Publish(new DiceRolledEvent(participantId, chosenDiceId, rawResult));
                }
                
                if (participantFightState.TurnState.IsConsumableChosen.Value)
                {
                    string chosenConsumableId = participantFightState.TurnState.SelectedConsumable.Value;
                    participantFightState.MarkConsumableUsed(chosenConsumableId, participantId);
                }
            }
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<PostRollEffectState>();
        }
        
        public void Exit(DuelContext context) { }
    }
}