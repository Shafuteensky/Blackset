using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 8. Пересчет счета битвы по актуальной истории бросков
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

        #region Inner
        
        private void ApplyParticipantsLastRollsToScore(DuelContext context)
        {
            foreach (var participant in context.Participants.Values)
            {
                int newScore = 0;
                FightParticipantState fightState = participant.FightState;

                newScore += GetFinalRollResultsSum(fightState);
                newScore += fightState.PersistentFightScoreModifier.Value;

                fightState.FightScore.Value = newScore;
            }
        }

        private int GetFinalRollResultsSum(FightParticipantState fightState)
        {
            int sumFinalRollsResult = 0;
            
            foreach (var roll in fightState.RollHistory.Values)
            {
                sumFinalRollsResult += roll.FinalResult;
            }

            return sumFinalRollsResult;
        }
        
        #endregion
    }
}