using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Extensions.Generics;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод целевого значения
    /// </summary>
    public sealed class TargetValueIndicator : AbstractText
    {
        private void OnEnable()
        {
            DuelController.Instance?.EventHub?.Subscribe<TargetValueSetEvent>(OnTargetValueSet);
        }

        private void OnTargetValueSet(TargetValueSetEvent handler)
        {
            DuelController.Instance?.DuelContext.TargetValue.TargetValue.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}