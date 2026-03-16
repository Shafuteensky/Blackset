using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод числа текущего броска битвы дуэли
    /// </summary>
    public sealed class CurrentThrowNumberIndicator : DuelDataTextIndicator
    {
        protected override void OnDuelInited(DuelInitedEvent handler)
        {
            duelContext.Progress.ThrowNumber.Subscribe(value => UpdateText(value.ToString()), true);
        }
    }
}