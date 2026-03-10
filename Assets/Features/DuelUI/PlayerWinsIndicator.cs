using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод количества побед игрока в битвах дуэли
    public sealed class PlayerWinsIndicator : ParticipantDataTextIndicator
    {
        protected override void OnDataInited(DuelInitedEvent handler)
        {
            duelContext.Participants[duelContext.PlayerId].FightsWon.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}