namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие объявления дайса участником 
    /// </summary>
    public struct DiceDeclarationCompletedEvent
    {
        public string DiceId;
        public string ParticipantOwnerId;

        public DiceDeclarationCompletedEvent(string diceId, string participantOwnerId)
        {
            DiceId = diceId;
            ParticipantOwnerId = participantOwnerId;
        }
    }
}