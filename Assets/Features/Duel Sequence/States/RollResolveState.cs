using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;
using UnityEngine;

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
                ProcessParticipant(context, participantId, diceRoller, duelScoreResolver);
            }
        }

        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<PostRollEffectState>();
        }

        public void Exit(DuelContext context) { }

        #region Inner
        
        private void ProcessParticipant(
            DuelContext context,
            string participantId,
            IDiceRollPipeline diceRoller,
            IDuelScoreResolver duelScoreResolver)
        {
            DuelParticipantState participant = context.Participants[participantId];
            FightParticipantState participantFightState = participant.FightState;

            if (participantFightState.TurnState.HasPassed.Value) return;

            ProcessDiceRoll(context, participantId, participantFightState, diceRoller, duelScoreResolver);
            ProcessConsumableUse(participantId, participantFightState);
        }

        private void ProcessDiceRoll(
            DuelContext context,
            string participantId,
            FightParticipantState participantFightState,
            IDiceRollPipeline diceRoller,
            IDuelScoreResolver duelScoreResolver)
        {
            if (!participantFightState.TurnState.IsDiceChosen.Value) return;

            string chosenDiceId = participantFightState.TurnState.SelectedDice.Value;
            int rawResult = diceRoller.RollDice(context, participantId, chosenDiceId, out bool isCrit);

            participantFightState.RegisterRawRollResult(chosenDiceId, rawResult);
            participantFightState.MarkThrow();
            participantFightState.MarkDiceUsed(chosenDiceId, participantId);

            duelScoreResolver.ResolveCrit(context, participantId, isCrit);
            eventHub.Publish(new DiceRolledEvent(participantId, chosenDiceId, rawResult));
        }

        private void ProcessConsumableUse(string participantId, FightParticipantState participantFightState)
        {
            if (!participantFightState.TurnState.IsConsumableChosen.Value) return;

            string chosenConsumableId = participantFightState.TurnState.SelectedConsumable.Value;
            participantFightState.MarkConsumableUsed(chosenConsumableId, participantId);
            
            eventHub.Publish(new ConsumableUsedEvent(participantId, chosenConsumableId));
        }
        
        #endregion
    }
}