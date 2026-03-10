using Blackset.Duel.Requests;

namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие награждения игрока
    /// </summary>
    public struct PlayerRewardedEvent
    {
        public readonly DuelRewards DuelRewards;

        public PlayerRewardedEvent(DuelRewards duelRewards)
        {
            DuelRewards = duelRewards;
        }
    }
}