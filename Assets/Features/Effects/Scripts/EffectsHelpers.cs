using Blackset.Data.Registries;
using Blackset.Duel.Participants;

namespace Blackset.Effects
{
    /// <summary>
    /// Вспомогательные методы для эффектов
    /// </summary>
    public static class EffectsHelpers
    {
        #region Цель применения эффектов

        /// <summary>
        /// Определить целевого участника по типу источника эффекта
        /// </summary>
        /// <param name="ownerParticipantId">Идентификатор владельца источника</param>
        /// <param name="turnState">Состояние участника на текущий ход</param>
        /// <param name="sourceKind">Тип источника эффекта</param>
        /// <returns>Идентификатор целевого участника</returns>
        public static string ResolveTargetParticipantId(
            string ownerParticipantId,
            TurnParticipantState turnState,
            EffectSourceKind sourceKind)
        {
            switch (sourceKind)
            {
                case EffectSourceKind.Consumable:
                {
                    if (turnState == null || string.IsNullOrEmpty(turnState.SelectedTargetParticipantId.Value))
                        return ownerParticipantId;
                    else
                        return turnState.SelectedTargetParticipantId.Value;
                }

                case EffectSourceKind.Dice:
                default:
                    return ownerParticipantId;
            }
        }

        #endregion
        
        #region Данные дайсов
        
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
        
        #endregion
    }
}