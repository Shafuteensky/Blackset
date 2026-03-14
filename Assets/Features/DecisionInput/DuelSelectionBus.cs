using System;
using Blackset.Duel.Targets;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Канал выбора предметов с визуалов
    /// </summary>
    /// <remarks>
    /// Служит посредником между <see cref="VisualItem"/> и <see cref="DuelInputPresenter"/>
    /// </remarks>
    public static class DuelSelectionBus
    {
        public static event Action<string, string> DiceSelected;
        public static event Action<string, string, ApplyTarget> ConsumableSelected;
        public static event Action PassRequested;

        /// <summary>
        /// Публикация запроса на выбор дайса
        /// </summary>
        /// <param name="ownerParticipantId">Идентификатор участника, выбравшего дайс</param>
        /// <param name="diceId">Идентификатор выбранного дайса в сборк</param>
        public static void PublishDiceSelected(string ownerParticipantId, string diceId)
        {
            DiceSelected?.Invoke(ownerParticipantId, diceId);
        }

        /// <summary>
        /// Публикация запроса на выбор расходника
        /// </summary>
        /// <param name="ownerParticipantId">Идентификатор участника, выбравшего дайс</param>
        /// <param name="consumableId">Идентификатор расходника</param>
        /// <param name="target">Цель применения</param>
        public static void PublishConsumableSelected(
            string ownerParticipantId,
            string consumableId,
            ApplyTarget target)
        {
            ConsumableSelected?.Invoke(ownerParticipantId, consumableId, target);
        }
        
        /// <summary>
        /// Публикация запроса на пас в ходу
        /// </summary>
        public static void PublishPassRequested()
        {
            PassRequested?.Invoke();
        }
    }
}