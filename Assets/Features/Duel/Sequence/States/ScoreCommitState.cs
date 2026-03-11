using Blackset.Duel.Context;
using Blackset.Duel.Modules;
using Blackset.Duel.Snapshots;
using Extensions.FiniteStateMachine;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// 8. Применение эффектов и учет счетов (повторяемое состояние)
    /// </summary>
    /// <remarks>
    /// - Построение снапшота хода боя для применения эффектов (резолва снапшота по правилам с учетом штормов и эффектов)
    /// - Обновление фактических счетов
    /// - Запись в историю
    /// </remarks>
    public class ScoreCommitState : BaseDuelState, IState<DuelContext>
    {
        public void Enter(DuelContext context)
        {
            // Применение эффектов
            // TODO Применение эффектов
            TurnSnapshot snapshot = new TurnSnapshot(context);
            IEffectsResolutionPipeline effectsResolver = modules.Get<IEffectsResolutionPipeline>();
            TurnSnapshot resolvedSnapshot = effectsResolver.Resolve(snapshot);
            
            // Актуализация фактических данных дуэли
            ISnapshotCommiter commiterDefault = modules.Get<ISnapshotCommiter>();
            commiterDefault.Commit(resolvedSnapshot, context);
        }
        
        public StateResult Tick(DuelContext context)
        {
            return StateResult.Switch<BattleCheckState>();
        }
        
        public void Exit(DuelContext context)
        {
            
        }
    }
}