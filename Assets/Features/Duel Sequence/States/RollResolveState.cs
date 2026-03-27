using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Participants;
using Blackset.Duel.Snapshots;
using Blackset.DuelEvents.EventTypes;
using Blackset.Effects;
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
            TurnSnapshot snapshot = context.Progress.CurrentTurnSnapshot;
            if (snapshot == null)
            {
                snapshot = new TurnSnapshot(context);
                context.Progress.CurrentTurnSnapshot = snapshot;
            }

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
                    int rollResult = diceRoller.RollDice(context, participantId, chosenDiceId, out bool isCrit);

                    snapshot.SetRawRollResult(participantId, rollResult);
                    snapshot.SetFinalRollResult(participantId, rollResult);

                    snapshot.AddUsageMutation(new UsageMutation(
                        participantId,
                        chosenDiceId,
                        EffectSourceKind.Dice,
                        participantId));

                    if (participantFightState.TurnState.IsConsumableChosen.Value)
                    {
                        snapshot.AddUsageMutation(new UsageMutation(
                            participantId,
                            participantFightState.TurnState.SelectedConsumable.Value,
                            EffectSourceKind.Consumable,
                            participantId));
                    }

                    duelScoreResolver.ResolveCrit(participant, isCrit);
                    eventHub.Publish(new DiceRolledEvent(participantId, chosenDiceId, rollResult));
                }
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