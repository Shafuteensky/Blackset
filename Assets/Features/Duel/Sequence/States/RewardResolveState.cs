using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Requests;
using Blackset.DuelEvents.EventTypes;
using Extensions.FiniteStateMachine;
using Features.Duel.Context;
using Features.Duel.Data.FightEnd;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 11. Расчет и выдача наград и опыта игроку
    /// </summary>
    /// <remarks>
    /// - Формирование награды по контракту с учетом бонусов и правил
    /// - Применение наград к инвентарю игрока и его мета-прогрессу
    /// </remarks>
    public class RewardResolveState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            DuelEndResult duelEndState = context.Progress.DuelResult;
            bool isPlayerWon = duelEndState.Winner.Value == FightWinner.Player;

            int playerDuelScore = context.Participants[context.PlayerId].DuelScore.Value;
            RewardRequest rewardRequest = new RewardRequest(isPlayerWon, context.Contract, playerDuelScore);
            IRewardService rewardService = modules.Get<IRewardService>();
            
            DuelRewards duelRewards = rewardService.BuildReward(rewardRequest);
            rewardService.ApplyResult(duelRewards);
            
            eventHub.Publish(new PlayerRewardedEvent(duelRewards));
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<DuelEndState>();
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}