using Blackset.Duel.Context;
using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод счета битвы участника
    /// <summary>
    public sealed class ParticipantFightScoreIndicator : ParticipantDataTextIndicator
    {
        protected override void OnDataInited(DuelInitedEvent handler, DuelContext duelContext)
        {
            duelContext.Participants[GetChosenParticipantId(duelContext)].FightState.FightScore.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}