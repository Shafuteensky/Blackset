using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод счета битвы игрока
    public sealed class PlayerScoreIndicator : ParticipantDataTextIndicator
    {
        protected override void OnDataInited(DuelInitedEvent handler)
        {
            duelContext.Participants[duelContext.OpponentId].FightState.Score.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}