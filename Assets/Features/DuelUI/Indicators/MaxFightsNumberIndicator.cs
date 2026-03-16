using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод числа максимума битв в дуэли
    /// </summary>
    public sealed class MaxFightsNumberIndicator : DuelDataTextIndicator
    {
        protected override void OnDuelInited(DuelInitedEvent handler)
        {
            UpdateText(duelContext.Rules.MaxFightsPerDuel.ToString());
        }
    }
}