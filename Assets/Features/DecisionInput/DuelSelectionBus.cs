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

        public static void PublishDiceSelected(string ownerParticipantId, string diceId)
        {
            DiceSelected?.Invoke(ownerParticipantId, diceId);
        }

        public static void PublishConsumableSelected(
            string ownerParticipantId,
            string consumableId,
            ApplyTarget target)
        {
            ConsumableSelected?.Invoke(ownerParticipantId, consumableId, target);
        }
    }
}