using Blackset.Data;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Inventories.Cells;
using UnityEngine;

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
        public static int GetCurrentDiceMaxValue(EffectApplyContext context)
        {
            int[] sideNumbers = GetCurrentDiceSideNumbers(context);
            if (sideNumbers == null || sideNumbers.Length == 0) return 0;

            int maxValue = sideNumbers[0];
            for (int i = 1; i < sideNumbers.Length; i++)
            {
                if (sideNumbers[i] > maxValue) maxValue = sideNumbers[i];
            }

            return maxValue;
        }

        /// <summary>
        /// Получить значения граней текущего выбранного дайса
        /// </summary>
        public static int[] GetCurrentDiceSideNumbers(EffectApplyContext context)
        {
            if (string.IsNullOrEmpty(context.SelectedDiceInstanceId)) return null;
            if (!context.DuelContext.Participants.TryGetValue(context.OwnerParticipantId, out var participant)) return null;

            var diceCell = participant.Sets.DiceSetInventory.GetById(context.SelectedDiceInstanceId);
            if (diceCell == null) return null;

            var diceData = GameData.Instance.GetDice(diceCell.Item.ItemId);
            var diceType = GameData.Instance.GetDiceType(diceCell.Item.ItemTypeId);
            if (diceData == null || diceType == null || diceData.NumbersConfig == null) return null;

            return diceData.NumbersConfig.GetSideNumbers(diceType);
        }
    }
}