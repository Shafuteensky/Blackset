namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие выбора дайса участником 
    /// </summary>
    public struct DiceSelectionCompletedEvent
    {
        public string DiceId;
        public string ParticipantOwnerId;

        public DiceSelectionCompletedEvent(string diceId, string participantOwnerId)
        {
            DiceId = diceId;
            ParticipantOwnerId = participantOwnerId;
        }
    }
}