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
            Debug.Log("ВХОД");
            if (!context.Snapshot.TryGetCurrentRoll(context.OwnerParticipantId, out RollHistoryEntry currentRoll))
                return false;
            Debug.Log(currentRoll);

            string opponentId = ResolveOpponentId(context);
            Debug.Log(opponentId);
            if (string.IsNullOrEmpty(opponentId))
                return false;

            if (!context.DuelContext.Participants.TryGetValue(opponentId, out var opponentParticipant))
                return false;
            Debug.Log(opponentParticipant);

            if (!opponentParticipant.FightState.TryGetLastRoll(out RollHistoryEntry opponentLastRoll))
                return false;
            Debug.Log(opponentLastRoll);

            currentRoll.FinalResult = opponentLastRoll.FinalResult;
            return true;
        }
    }
}