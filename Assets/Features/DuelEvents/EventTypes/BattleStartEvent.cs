namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие начала битвы
    /// </summary>
    public struct BattleStartEvent
    {
        public readonly int BattleNumber;

        public BattleStartEvent(int battleNumber)
        {
            BattleNumber = battleNumber;
        }
    }
}