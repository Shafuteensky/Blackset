using Blackset.Duel.Context;
using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод счета битвы игрока
    /// <summary>
    public sealed class PlayerScoreIndicator : ParticipantDataTextIndicator
    {
        protected override void OnDataInited(DuelInitedEvent handler, DuelContext duelContext)
        {
            duelContext.Participants[duelContext.PlayerId].FightState.Score.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}