using Blackset.Duel.Snapshots;

namespace Features.Duel.Modules
{
    /// <summary>
    /// Пайплайн применения эффектов дайсов и расходников
    /// </summary>
    public interface IEffectResolutionPipeline
    {
        /// <summary>
        /// Применить эффекты
        /// </summary>
        /// <param name="snapshot">Снапшот дуэли на начало хода</param>
        /// <returns>Резолвеный снапшот с примененными эффектами</returns>
        public TurnSnapshot Resolve(TurnSnapshot snapshot);
    }
}