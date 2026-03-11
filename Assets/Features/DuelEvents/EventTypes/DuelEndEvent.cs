using Features.Duel.Context;

namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие окончания дуэли
    /// </summary>
    public struct DuelEndEvent
    {
        public readonly DuelEndResult DuelEndResult;
        
        public DuelEndEvent(DuelEndResult duelEndResult)
        {
            DuelEndResult = duelEndResult;
        }
    }
}