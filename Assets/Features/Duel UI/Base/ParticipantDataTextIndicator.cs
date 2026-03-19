using Blackset.Duel.Context;
using Blackset.DuelEvents.EventTypes;
using Extensions.Log;
using Features.Duel.Data.FightEnd;
using UnityEngine;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Подписка для обновления текста данных участников дуэли после инициализации дуэли
    /// <summary>
    public abstract class ParticipantDataTextIndicator : DuelDataTextIndicator
    {
        [Header("Участник"), Space]
        [SerializeField] protected FightWinner participant = FightWinner.Player;
        
        protected override void OnDuelInited(DuelInitedEvent handler)
        {
            if (duelContext == null)
            {
                ServiceDebug.LogError("Данные дуэли отсутствуют, индикатор неактивен");
                return;
            }
            
            OnDataInited(handler, duelContext);
        }

        protected string GetChosenParticipantId(DuelContext duelContext)
        {
            string participantId;
            
            if (participant == FightWinner.Opponent) participantId = duelContext.OpponentId;
            else participantId = duelContext.PlayerId;
            
            return participantId;
        }

        protected abstract void OnDataInited(DuelInitedEvent handler, DuelContext duelContext);
    }
}