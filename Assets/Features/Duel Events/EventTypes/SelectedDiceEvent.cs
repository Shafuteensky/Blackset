namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие выбора дайса участником 
    /// </summary>
    public struct SelectedDiceEvent
    {
        public string DiceId;
        public string ParticipantOwnerId;

        public SelectedDiceEvent(string diceId, string participantOwnerId)
        {
            DiceId = diceId;
            ParticipantOwnerId = participantOwnerId;
        }
    }
}