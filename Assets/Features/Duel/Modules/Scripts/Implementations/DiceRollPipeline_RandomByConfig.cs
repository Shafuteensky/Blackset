using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Inventories.Cells;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Случайный бросок дайса с учетом конфигурации граней <see cref="SideNumbersConfig"/>
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(DiceRollPipeline_RandomByConfig),
        menuName = "Blackset/Duel/Modules/" + nameof(DiceRollPipeline_RandomByConfig))]
    public class DiceRollPipeline_RandomByConfig : BaseDuelModule, IDiceRollPipeline
    {
        public int RollDice(DuelContext context, string participantId, string diceId, out bool isCrit)
        {
            isCrit = false;
            if (string.IsNullOrEmpty(diceId)) return 0;
            
            DuelParticipantState participantState = context.Participants[participantId];
            InventoryCell diceInSet = participantState.Sets.DiceSetInventory.GetById(diceId);
            string diceTypeToRoll = diceInSet.Item.ItemTypeId;
            
            GameData gameData = GameData.Instance;
            DiceType diceType = gameData.GetDiceType(diceTypeToRoll);
            DiceData dice = gameData.GetDice(diceInSet.Item.ItemId);
                
            int[] sides = dice.NumbersConfig.GetSideNumbers(diceType);
            int index = Random.Range(0, sides.Length);
            int rollResult = sides[index];
            
            int sidesNumber = diceType.SidesNumber;
            if (rollResult >= sidesNumber) isCrit = true;
            
            return rollResult;
        }
        
        public int RollChosenDice(DuelContext context, string participantId, out bool isCrit)
        {
            string chosenDiceId = context.Participants[participantId].FightState.TurnState.SelectedDice.Value.ItemId;
            int rollResult = RollDice(context, participantId, chosenDiceId, out isCrit);
            return rollResult;
        }
    }
}