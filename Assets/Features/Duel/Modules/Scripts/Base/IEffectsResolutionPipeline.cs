using Blackset.Duel.Snapshots;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Пайплайн применения эффектов дайсов и расходников
    /// </summary>
    public interface IEffectsResolutionPipeline : IDuelModuleInterface
    {
        /// <summary>
        /// Применить эффекты
        /// </summary>
        /// <param name="snapshot">Снапшот дуэли на начало хода</param>
        /// <returns>Резолвеный снапшот с примененными эффектами</returns>
        public TurnSnapshot Resolve(TurnSnapshot snapshot);
    }
}