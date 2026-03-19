using Blackset.Duel.Snapshots;

namespace Blackset.Duel.Modules
{
    /// <summary>
    /// Резолвер роллов и эффектов дайсов/расходников (пайпалайн обновления счетов)
    /// </summary>
    public interface IScoreUpdatePipeline : IDuelModuleInterface
    {
        /// <summary>
        /// Применить эффекты
        /// </summary>
        /// <param name="snapshot">Снапшот дуэли на начало хода</param>
        /// <returns>Резолвеный снапшот с примененными эффектами</returns>
        public TurnSnapshot Resolve(TurnSnapshot snapshot);
    }
}