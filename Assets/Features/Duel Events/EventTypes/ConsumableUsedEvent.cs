namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие использования расходника
    /// </summary>
    public struct ConsumableUsedEvent
    {
        public readonly string ParticipantOwnerId;
        public readonly string ParticipantTargetId;
        public readonly string ChosenConsumableId;
        
        public ConsumableUsedEvent(string participantOwnerId, string participantTargetId, string chosenConsumableId)
        {
            ParticipantOwnerId = participantOwnerId;
            ParticipantTargetId = participantTargetId;
            ChosenConsumableId = chosenConsumableId;
        }
    }
}