namespace Blackset.DuelEvents.EventTypes
{
    /// <summary>
    /// Событие броска дайсов и получения результатов
    /// </summary>
    public struct DiceRolledEvent
    {
        public string ParticipantId;
        public string ChosenDiceId;
        public int RollResult;
        
        public DiceRolledEvent(string participantId, string chosenDiceId, int rollResult)
        {
            ParticipantId = participantId;
            ChosenDiceId = chosenDiceId;
            RollResult = rollResult;
        }
    }
}