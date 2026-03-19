using Blackset.Duel.Snapshots;
using UnityEngine;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Стандартный резолвер роллов и эффектов дайсов/расходников (пайпалайн обновления счетов)
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(ScoreUpdatePipeline_Default),
        menuName = "Blackset/Duel/Modules/" + nameof(ScoreUpdatePipeline_Default))]
    public class ScoreUpdatePipeline_Default : BaseDuelModule, IScoreUpdatePipeline
    {
        public TurnSnapshot Resolve(TurnSnapshot snapshot)
        {
            // Сырые результаты бросков в счета
            // TODO Обновить под применение ЭФФЕКТОВ: добавить FinalRollResults в FightState участника
            foreach (var participantResults in snapshot.ParticipantRawRollResults)
            {
                int score = 0;
                
                foreach (int rollResult in participantResults.Value.Values)
                    score += rollResult;
                
                snapshot.ParticipantScores[participantResults.Key] = score;
            }
            
            return snapshot;
        }
    }
}