namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие выбора расходника участником 
    /// </summary>
    public struct ConsumableSelectionCompletedEvent
    {
        public string ConsumableId;
        public string ParticipantOwnerId;

        public ConsumableSelectionCompletedEvent(string consumableId, string participantOwnerId)
        {
            ConsumableId = consumableId;
            ParticipantOwnerId = participantOwnerId;
        }
    }
}