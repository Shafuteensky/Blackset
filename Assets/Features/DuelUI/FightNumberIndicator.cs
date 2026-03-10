using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод числа текущей битвы дуэли
    /// </summary>
    public sealed class FightNumberIndicator : DuelDataTextIndicator
    {
        protected override void OnDuelInited(DuelInitedEvent handler)
        {
            DuelController.Instance?.DuelContext.Progress.FightNumber.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}