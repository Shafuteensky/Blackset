using Features.Duel.Context;

namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие окончания дуэли (завершение всех битв)
    /// </summary>
    public struct DuelFinishEvent
    {
        public readonly DuelEndResult DuelEndResult;
        
        public DuelFinishEvent(DuelEndResult duelEndResult)
        {
            DuelEndResult = duelEndResult;
        }
    }
}