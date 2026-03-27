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
        
        public void ResolveDuelWin(DuelContext context, FightEndResult fightEndResult, TurnSnapshot snapshot)
        {
            foreach (var participant in context.Participants.Values)
            {
                int score = 0;

                if (fightEndResult.WinnerId == participant.ParticipantId)
                {
                    score += DuelScoreConfig.BattleResult.battleWinScore;
                }

                snapshot.AddDuelScore(participant.ParticipantId, Mathf.Max(0, score));
            }
        }

        public void ResolveUnusedItems(DuelContext context, TurnSnapshot snapshot)
        {
            foreach (var participant in context.Participants.Values)
            {
                int unusedDicesCount = participant.Sets.DiceSetInventory.Data.Count -
                                       participant.FightState.GetUsedDices().Count;

                int score = unusedDicesCount * DuelScoreConfig.UnusedItems.unusedDiceScore;
                snapshot.AddDuelScore(participant.ParticipantId, Mathf.Max(0, score));
                
                int unusedConsumablesCount = participant.Sets.ConsumableSetInventory.Data.Count -
                                             participant.FightState.GetUsedConsumables().Count;

                score = unusedConsumablesCount * DuelScoreConfig.UnusedItems.unusedConsumableScore;
                snapshot.AddDuelScore(participant.ParticipantId, Mathf.Max(0, score));
            }
        }

        public void ResolveCrit(string participantId, bool isCrit, TurnSnapshot snapshot)
        {
            if (!isCrit)
            {
                return;
            }
            
            snapshot.AddDuelScore(participantId, DuelScoreConfig.Precision.diceCriticalScore);
        }

        public void ResolveExactTargetHit(string participantId, bool isCrit, TurnSnapshot snapshot)
        {
            if (!isCrit)
            {
                return;
            }
            
            snapshot.AddDuelScore(participantId, DuelScoreConfig.Precision.exactTargetZoneHitScore);
        }

        public void ResolveHonesty(string participantId, string declared, string chosen, TurnSnapshot snapshot)
        {
            int score = declared == chosen
                ? DuelScoreConfig.Declaration.honestDeclarationScore
                : DuelScoreConfig.Declaration.bluffDeclarationScore;

            snapshot.AddDuelScore(participantId, Mathf.Max(0, score));
        }

        public void ResolveSuccessfulBluff(string participantId, bool isCrit, TurnSnapshot snapshot)
        {
            if (!isCrit)
            {
                return;
            }
            
            snapshot.AddDuelScore(participantId, DuelScoreConfig.Declaration.successfulBluffScore);
        }

        public void ResolveCaughtBluff(string participantId, bool isCrit, TurnSnapshot snapshot)
        {
            if (!isCrit)
            {
                return;
            }
            
            snapshot.AddDuelScore(participantId, DuelScoreConfig.Declaration.caughtEnemyBluffScore);
        }
    }
}