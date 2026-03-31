namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие использования расходника
    /// </summary>
    public struct ConsumableUsedEvent
    {
        public string ParticipantId;
        public string ChosenConsumableId;
        
        public ConsumableUsedEvent(string participantId, string chosenConsumableId)
        {
            ParticipantId = participantId;
            ChosenConsumableId = chosenConsumableId;
        }
    }
}