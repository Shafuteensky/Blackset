using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод количества побед соперника в битвах дуэли
    public sealed class OpponentWinsIndicator : ParticipantDataTextIndicator
    {
        protected override void OnDataInited(DuelInitedEvent handler)
        { 
            duelContext.Participants[duelContext.OpponentId].FightsWon.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}