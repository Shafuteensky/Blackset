using Blackset.Duel.Context;
using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод количества побед игрока в битвах дуэли
    /// <summary>
    public sealed class PlayerWinsIndicator : ParticipantDataTextIndicator
    {
        protected override void OnDataInited(DuelInitedEvent handler, DuelContext duelContext)
        {
            duelContext.Participants[duelContext.PlayerId].FightsWon.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}