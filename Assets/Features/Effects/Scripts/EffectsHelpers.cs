using Blackset.Data.Registries;
using Blackset.Duel.Participants;
using Blackset.Inventories.Cells;

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
        
        /// <summary>
        /// Получить максимальное значение граней текущего выбранного дайса цели
        /// </summary>
        public static int GetTargetCurrentDiceMaxValue(EffectApplyContext context)
        {
            int[] sideNumbers = GetTargetCurrentDiceSideNumbers(context);
            if (sideNumbers == null || sideNumbers.Length == 0) return 0;

            int maxValue = sideNumbers[0];
            for (int i = 1; i < sideNumbers.Length; i++)
            {
                if (sideNumbers[i] > maxValue) maxValue = sideNumbers[i];
            }

            return maxValue;
        }
        
        /// <summary>
        /// Получить значения граней текущего выбранного дайса цели
        /// </summary>
        public static int[] GetTargetCurrentDiceSideNumbers(EffectApplyContext context)
        {
            if (!context.DuelContext.Participants.TryGetValue(context.TargetParticipantId, out var participant))
                return null;
            if (participant.FightState == null || participant.FightState.TurnState == null)
                return null;

            string targetDiceInstanceId = participant.FightState.TurnState.SelectedDice.Value;

            return GetDiceSideNumbers(
                context.DuelContext,
                context.TargetParticipantId,
                targetDiceInstanceId);
        }

        /// <summary>
        /// Получить значения граней указанного дайса указанного участника
        /// </summary>
        public static int[] GetDiceSideNumbers(
            Duel.Context.DuelContext duelContext,
            string participantId,
            string diceInstanceId)
        {
            if (string.IsNullOrEmpty(participantId) || string.IsNullOrEmpty(diceInstanceId))
                return null;
            if (!duelContext.Participants.TryGetValue(participantId, out var participant))
                return null;

            InventoryCell diceCell = participant.Sets.DiceSetInventory.GetById(diceInstanceId);
            if (diceCell == null) return null;

            var diceData = GameData.Instance.GetDice(diceCell.Item.ItemId);
            var diceType = GameData.Instance.GetDiceType(diceCell.Item.ItemTypeId);
            if (diceData == null || diceType == null || diceData.NumbersConfig == null)
                return null;

            return diceData.NumbersConfig.GetSideNumbers(diceType);
        }

        #endregion
    }
}