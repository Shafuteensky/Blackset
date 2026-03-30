using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Duel.Rolls;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 8. Коммит рабочего снапшота текущего броска
    /// </summary>
    public class ScoreCommitState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            eventHub.Publish(new EffectsResolvedEvent(context));
            ApplyParticipantsLastRollsToScore(context);
        }

        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<BattleCheckState>();
        }

        public void Exit(DuelContext context) { }

        private void ApplyParticipantsLastRollsToScore(DuelContext context)
        {
            foreach (var participant in context.Participants.Values)
            {
                FightParticipantState fightState = participant.FightState;
                if (fightState.TurnState.IsDiceChosen.Value &&
                    fightState.TryGetLastRoll(out RollHistoryEntry rollEntry))
                {
                    fightState.AddScore(rollEntry.FinalResult);
                }
            }
        }

    }
}