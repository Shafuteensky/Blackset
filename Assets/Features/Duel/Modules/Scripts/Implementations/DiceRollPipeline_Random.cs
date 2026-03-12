using Blackset.Data.Registries;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Случайный бросок дайса
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(DiceRollPipeline_Random),
        menuName = "Blackset/Duel/Modules/" + nameof(DiceRollPipeline_Random))]
    public class DiceRollPipeline_Random : BaseDuelModule, IDiceRollPipeline
    {
        public int RollDice(DuelContext context, string participantId, string diceId)
        {
            DuelParticipantState participantState = context.Participants[participantId];
            string diceTypeToRoll = participantState.Sets.DiceSetInventory.GetById(diceId).ItemTypeId;
            
            int sidesNumber = GameData.Instance.GetDiceType(diceTypeToRoll).SidesNumber;
            int rollResult = Random.Range(1, sidesNumber);
            
            return rollResult;
        }
        
        public int RollChosenDice(DuelContext context, string participantId)
        {
            string chosenDiceId = context.Participants[participantId].FightState.TurnState.ChosenDice.Value;
            int rollResult = RollDice(context, participantId, chosenDiceId);
            return rollResult;
        }
    }
}