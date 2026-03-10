using Blackset.Duel.Context;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Extensions.Log;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Подписка для обновления текста данных участников дуэли после инициализации дуэли
    /// <summary>
    public abstract class ParticipantDataTextIndicator : DuelDataTextIndicator
    {
        protected DuelContext duelContext;
        
        protected override void OnDuelInited(DuelInitedEvent handler)
        {
            duelContext = DuelController.Instance?.DuelContext;
            if (duelContext == null)
            {
                ServiceDebug.LogError("Данные дуэли отсутствуют, индикатор неактивен");
                return;
            }
            
            OnDataInited(handler);
        }

        protected abstract void OnDataInited(DuelInitedEvent handler);
    }
}