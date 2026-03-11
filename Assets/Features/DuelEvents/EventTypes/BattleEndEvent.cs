using Blackset.Duel.Context;
using Features.Duel.Data.FightEnd;

namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие окончания битвы
    /// </summary>
    public struct BattleEndEvent
    {
        public readonly DuelContext DuelContext;
        public readonly FightEndResult FightEndResult;

        public BattleEndEvent(DuelContext duelContext, FightEndResult fightEndResult)
        {
            DuelContext = duelContext;
            FightEndResult = fightEndResult;
        }
    }
}