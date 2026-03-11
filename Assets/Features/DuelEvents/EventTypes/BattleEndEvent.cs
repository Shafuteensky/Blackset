using Blackset.Duel.Context;

namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие окончания битвы
    /// </summary>
    public struct BattleEndEvent
    {
        public readonly DuelContext DuelContext;

        public BattleEndEvent(DuelContext duelContext)
        {
            DuelContext = duelContext;
        }
    }
}