namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие выбора расходника участником 
    /// </summary>
    public struct SelectedConsumableEvent
    {
        public string ConsumableId;
        public string ParticipantOwnerId;

        public SelectedConsumableEvent(string consumableId, string participantOwnerId)
        {
            ConsumableId = consumableId;
            ParticipantOwnerId = participantOwnerId;
        }
    }
}