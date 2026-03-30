using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Inventories.Cells;

namespace Blackset.Effects
{
    /// <summary>
    /// Вспомогательные методы для эффектов
    /// </summary>
    public static class EffectsHelpers
    {
        /// <summary>
        /// Получить максимальное значение граней выбранного дайса
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int GetCurrentDiceMaxValue(EffectApplyContext context)
        {
            if (string.IsNullOrEmpty(context.SelectedDiceInstanceId))
                return 0;
            if (!context.DuelContext.Participants.TryGetValue(context.OwnerParticipantId, out var participant))
                return 0;
            InventoryCell diceCell = participant.Sets.DiceSetInventory.GetById(context.SelectedDiceInstanceId);
            if (diceCell == null)
                return 0;

            GameData gameData = GameData.Instance;
            DiceData diceData = gameData.GetDice(diceCell.Item.ItemId);
            DiceType diceType = gameData.GetDiceType(diceCell.Item.ItemTypeId);
            
            if (diceData == null || diceType == null || diceData.NumbersConfig == null)
                return 0;

            int[] sideNumbers = diceData.NumbersConfig.GetSideNumbers(diceType);
            if (sideNumbers == null || sideNumbers.Length == 0)
                return 0;

            int maxValue = sideNumbers[0];
            for (int i = 1; i < sideNumbers.Length; i++)
            {
                if (sideNumbers[i] > maxValue)
                    maxValue = sideNumbers[i];
            }

            return maxValue;
        }
    }
}