namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие объявления дайса участником 
    /// </summary>
    public struct DeclaredDiceEvent
    {
        public string DiceId;
        public string ParticipantOwnerId;

        public DeclaredDiceEvent(string diceId, string participantOwnerId)
        {
            DiceId = diceId;
            ParticipantOwnerId = participantOwnerId;
        }
    }
}