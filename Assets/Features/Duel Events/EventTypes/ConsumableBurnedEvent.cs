namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие уничтожения SingleUse расходника
    /// </summary>
    public struct ConsumableBurnedEvent
    {
        public string ParticipantId;
        public string ChosenConsumableId;
        
        public ConsumableBurnedEvent(string participantId, string chosenConsumableId)
        {
            ParticipantId = participantId;
            ChosenConsumableId = chosenConsumableId;
        }
    }
}