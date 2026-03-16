using Blackset.Duel.Context;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Extensions.Generics;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Подписка для обновления текста после инициализации дуэли
    /// </summary>
    public abstract class DuelDataTextIndicator : AbstractText
    {
        protected DuelContext duelContext => DuelController.Instance?.DuelContext;
        
        protected virtual void OnEnable()
        {
            DuelController.Instance?.EventHub?.Subscribe<DuelInitedEvent>(OnDuelInited);
        }

        protected abstract void OnDuelInited(DuelInitedEvent handler);
    }
}