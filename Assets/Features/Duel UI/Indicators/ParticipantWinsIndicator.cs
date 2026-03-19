using Blackset.Duel.Context;
using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод количества побед участника в битвах дуэли
    /// <summary>
    public sealed class ParticipantWinsIndicator : ParticipantDataTextIndicator
    {
        protected override void OnDataInited(DuelInitedEvent handler, DuelContext duelContext)
        {
            duelContext.Participants[GetChosenParticipantId(duelContext)].FightsWon.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}