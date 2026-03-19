using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод числа текущей битвы дуэли
    /// </summary>
    public sealed class CurrentFightNumberIndicator : DuelDataTextIndicator
    {
        protected override void OnDuelInited(DuelInitedEvent handler)
        {
            duelContext.Progress.FightNumber.Subscribe(value => UpdateText(value.ToString()), true);
        }
    }
}