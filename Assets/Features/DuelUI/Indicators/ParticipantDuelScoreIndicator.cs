using Blackset.Duel.Context;
using Blackset.DuelEvents.EventTypes;

namespace Blackset.DuelUI
{
    /// <summary>
    /// Вывод счета дуэли участника
    /// <summary>
    public sealed class ParticipantDuelScoreIndicator : ParticipantDataTextIndicator
    {
        protected override void OnDataInited(DuelInitedEvent handler, DuelContext duelContext)
        {
            duelContext.Participants[GetChosenParticipantId(duelContext)].DuelScore.Subscribe(
                value => UpdateText(value.ToString()), true);
        }
    }
}