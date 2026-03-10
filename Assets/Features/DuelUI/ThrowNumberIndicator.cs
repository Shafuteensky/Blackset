using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод числа текущего броска битвы дуэли
    /// </summary>
    public sealed class ThrowNumberIndicator : DuelDataTextIndicator
    {
        protected override void OnDuelInited(DuelInitedEvent handler)
        {
            DuelController.Instance?.DuelContext.Progress.ThrowNumber.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}