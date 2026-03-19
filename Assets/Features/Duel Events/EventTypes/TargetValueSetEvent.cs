namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие установки целевого значения
    /// </summary>
    public struct TargetValueSetEvent
    {
        public readonly int TargetValue;

        public TargetValueSetEvent(int targetValue)
        {
            TargetValue = targetValue;
        }
    }
}