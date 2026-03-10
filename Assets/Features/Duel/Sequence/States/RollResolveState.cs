using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;

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
            // Броски дайсов
            IDiceRollPipeline diceRoller = modules.Get<IDiceRollPipeline>();
            foreach (string participantId in context.Participants.Keys)
            {
                string chosenDiceId = context.Participants[participantId].FightState.TurnState.ChosenDice.Value;
                int rollResult = diceRoller.RollDice(context, participantId, chosenDiceId);
                context.Participants[participantId].FightState.RegisterRawRollResult(chosenDiceId, rollResult); 
            }
            
            eventHub.Publish(new DicesRolledEvent());
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