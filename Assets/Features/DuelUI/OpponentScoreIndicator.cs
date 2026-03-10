using Blackset.Duel.Context;
using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод счета битвы соперника
    /// <summary>
    public sealed class OpponentScoreIndicator : ParticipantDataTextIndicator
    {
        protected override void OnDataInited(DuelInitedEvent handler, DuelContext duelContext)
        {
            duelContext.Participants[duelContext.OpponentId].FightState.Score.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}