using Blackset.Duel.Modules;
using Extensions.Log;

namespace Features.Duel.Sequence.States
{
    /// <summary>
    /// Базовое состояние дуэли со входным параметром
    /// </summary>
    public class BaseDuelState
    {
        private DuelModuleRegistry duelModules;
        
        /// <summary>
        /// Инициализация данных состояния
        /// </summary>
        /// <param name="modules">Реестр модулей обработки данных дуэли</param>
        public void Initialize(DuelModuleRegistry modules)
        {
            ServiceGuard.NotNull(modules, nameof(modules));
            duelModules = modules;
        }
    }
}