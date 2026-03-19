using Blackset.Duel.Context;

namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие резолва роллов и эффектов (фиксация счетов)
    /// </summary>
    public struct EffectsResolvedEvent
    {
        public DuelContext DuelContext;
        
        public EffectsResolvedEvent(DuelContext duelContext)
        {
            DuelContext = duelContext;
        }
    }
}