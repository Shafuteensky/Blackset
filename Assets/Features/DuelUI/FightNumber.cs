using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Extensions.Generics;

namespace Features.DuelUI
{
    /// <summary>
    /// Вывод числа
    /// </summary>
    public sealed class FightNumber : AbstractText
    {
        private void OnEnable()
        {
            DuelController.Instance.EventHub.Subscribe<DuelInitedEvent>(OnDuelInited);
        }

        private void OnDuelInited(DuelInitedEvent handler)
        {
            DuelController.Instance.DuelContext.Progress.FightNumber.Subscribe(value => UpdateText(value.ToString()), true);
        }
    }
}