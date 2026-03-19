using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод числа максимума бросков битвы дуэли
    /// </summary>
    public sealed class MaxThrowsNumberIndicator : DuelDataTextIndicator
    {
        protected override void OnDuelInited(DuelInitedEvent handler)
        {
            UpdateText(duelContext.Rules.MaxThrowsPerFight.ToString());
        }
    }
}