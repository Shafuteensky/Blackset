using Blackset.Data.Registries;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
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
        GameData gameData => GameData.Instance;
        DuelScoreConfig duelScoreConfig => gameData.DuelScoreConfig;
        
        public void ResolveDuelWin(DuelContext context, FightEndResult fightEndResult)
        {
            foreach (var participant in context.Participants.Values)
            {
                int score = 0;

                // Победа в битве
                if (fightEndResult.WinnerId == participant.ParticipantId)
                    score += duelScoreConfig.BattleResult.battleWinScore;

                // TODO Учитывать квиквин, камбек
                // if (false)
                //     score += duelScoreConfig.BattleResult.quickWinScore;
                // if (false)
                //     score += duelScoreConfig.BattleResult.comebackWinScore;

                participant.DuelScore.Value += Mathf.Max(0, score);
            }
        }

        public void ResolveUnusedItems(DuelContext context)
        {
            foreach (var participant in context.Participants.Values)
            {
                int unusedDicesCount = participant.Sets.DiceSetInventory.Data.Count -
                                       participant.FightState.GetUsedDices().Count;
                    
                int score = unusedDicesCount * duelScoreConfig.UnusedItems.unusedDiceScore;
                participant.DuelScore.Value += Mathf.Max(0, score);
                
                int unusedConsumablesCount = participant.Sets.ConsumableSetInventory.Data.Count -
                                             participant.FightState.GetUsedConsumables().Count;
                
                score = unusedConsumablesCount * duelScoreConfig.UnusedItems.unusedConsumableScore;
                participant.DuelScore.Value += Mathf.Max(0, score);
            }
        }

        public void ResolveCrit(DuelParticipantState participant, bool isCrit)
        {
            if (!isCrit) return;
            
            participant.DuelScore.Value += duelScoreConfig.Precision.diceCriticalScore;
        }

        public void ResolveExactTargetHit(DuelParticipantState participant, bool isCrit)
        {
            if (!isCrit) return;
            
            participant.DuelScore.Value += duelScoreConfig.Precision.exactTargetZoneHitScore;
        }

        public void ResolveHonesty(DuelParticipantState participant, string declared, string chosen)
        {
            int score = 0;
            if (declared == chosen)
                score += duelScoreConfig.Declaration.honestDeclarationScore;
            else
                score += duelScoreConfig.Declaration.bluffDeclarationScore;

            participant.DuelScore.Value += Mathf.Max(0, score);
        }

        public void ResolveSuccessfulBluff(DuelParticipantState participant, bool isCrit)
        {
            if (!isCrit) return;
            
            participant.DuelScore.Value += duelScoreConfig.Declaration.successfulBluffScore;
        }

        public void ResolveCaughtBluff(DuelParticipantState participant, bool isCrit)
        {
            if (!isCrit) return;
            
            participant.DuelScore.Value += duelScoreConfig.Declaration.caughtEnemyBluffScore;
        }
    }
}