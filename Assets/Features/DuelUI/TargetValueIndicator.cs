using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод целевого значения
    /// </summary>
    public sealed class TargetValueIndicator : DuelDataTextIndicator
    {
        protected override void OnDuelInited(DuelInitedEvent handler)
        {
            DuelController.Instance?.DuelContext.TargetValue.TargetValue.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}