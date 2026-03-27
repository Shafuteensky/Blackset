using Blackset.Duel.Rolls;
using UnityEngine;

namespace Blackset.Effects
{
    /// <summary>
    /// Эффект зеркалирования последнего финального броска соперника
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(MirrorEffect),
        menuName = "Blackset/Effects/" + nameof(MirrorEffect))]
    public class MirrorEffect : AbstractEffect
    {
        /// <summary>
        /// Применить внутреннюю логику эффекта
        /// </summary>
        protected override bool ApplyInternal(EffectApplyContext context)
        {
            if (!context.Snapshot.TryGetCurrentRoll(context.OwnerParticipantId, out RollHistoryEntry currentRoll))
                return false;

            string opponentId = ResolveOpponentId(context);
            if (string.IsNullOrEmpty(opponentId))
                return false;

            if (!context.DuelContext.Participants.TryGetValue(opponentId, out var opponentParticipant))
                return false;

            if (!opponentParticipant.FightState.TryGetLastRoll(out RollHistoryEntry opponentLastRoll))
                return false;

            currentRoll.FinalResult = opponentLastRoll.FinalResult;
            return true;
        }
    }
}