using Blackset.Duel.Context;

namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие начала битвы
    /// </summary>
    public struct BattleStartEvent
    {
        public readonly DuelContext DuelContext;

        public BattleStartEvent(DuelContext duelContext)
        {
            DuelContext = duelContext;
        }
    }
}