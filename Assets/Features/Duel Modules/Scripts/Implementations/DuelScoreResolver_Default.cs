using Blackset.Data.Registries;
using Blackset.Duel.Context;
using Blackset.Duel.Snapshots;
using Blackset.DuelScore;
using Features.Duel.Data.FightEnd;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Стандартный резолвер счетов дуэли участников дуэли
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(DuelScoreResolver_Default),
        menuName = "Blackset/Duel/Modules/" + nameof(DuelScoreResolver_Default))]
    public class DuelScoreResolver_Default : BaseDuelModule, IDuelScoreResolver
    {
        private GameData GameData => GameData.Instance;
        private DuelScoreConfig DuelScoreConfig => GameData.DuelScoreConfig;
        
        public void ResolveDuelWin(DuelContext context, FightEndResult fightEndResult)
        {
            foreach (var participant in context.Participants.Values)
            {
                int score = 0;

                if (fightEndResult.WinnerId == participant.ParticipantId)
                {
                    score += DuelScoreConfig.BattleResult.battleWinScore;
                }

                context.Participants[participant.ParticipantId].AddDuelScore(score);
            }
        }

        public void ResolveUnusedItems(DuelContext context)
        {
            foreach (var participant in context.Participants.Values)
            {
                int unusedDicesCount = participant.Sets.DiceSetInventory.Data.Count -
                                       participant.FightState.GetUsedDices().Count;

                int score = unusedDicesCount * DuelScoreConfig.UnusedItems.unusedDiceScore;
                context.Participants[participant.ParticipantId].AddDuelScore(score);
                
                int unusedConsumablesCount = participant.Sets.ConsumableSetInventory.Data.Count -
                                             participant.FightState.GetUsedConsumables().Count;
                score = unusedConsumablesCount * DuelScoreConfig.UnusedItems.unusedConsumableScore;
                context.Participants[participant.ParticipantId].AddDuelScore(score);
            }
        }

        public void ResolveCrit(DuelContext context, string participantId, bool isCrit)
        {
            if (!isCrit) return;
            context.Participants[participantId].AddDuelScore(DuelScoreConfig.Precision.diceCriticalScore);
        }

        public void ResolveExactTargetHit(DuelContext context, string participantId, bool isCrit)
        {
            if (!isCrit) return;
            context.Participants[participantId].AddDuelScore(DuelScoreConfig.Precision.exactTargetZoneHitScore);
        }

        public void ResolveHonesty(DuelContext context, string participantId, string declared, string chosen)
        {
            int score = declared == chosen
                ? DuelScoreConfig.Declaration.honestDeclarationScore
                : DuelScoreConfig.Declaration.bluffDeclarationScore;

            context.Participants[participantId].AddDuelScore(score);
        }

        public void ResolveSuccessfulBluff(DuelContext context, string participantId, bool isCrit)
        {
            if (!isCrit) return;
            context.Participants[participantId].AddDuelScore(DuelScoreConfig.Declaration.successfulBluffScore);
        }

        public void ResolveCaughtBluff(DuelContext context, string participantId, bool isCrit)
        {
            if (!isCrit) return;
            context.Participants[participantId].AddDuelScore(DuelScoreConfig.Declaration.successfulBluffScore);
        }
    }
}