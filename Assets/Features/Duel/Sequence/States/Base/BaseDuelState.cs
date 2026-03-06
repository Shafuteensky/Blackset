using Blackset.Duel.Modules;
using Extensions.Log;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// Базовое состояние дуэли со входным параметром
    /// </summary>
    public class BaseDuelState
    {
        protected DuelModuleRegistry modules;
        
        /// <summary>
        /// Инициализация данных состояния
        /// </summary>
        /// <param name="modules">Реестр модулей обработки данных дуэли</param>
        public void Initialize(DuelModuleRegistry modulesRegistry)
        {
            ServiceGuard.NotNull(modules, nameof(modules));
            modules = modulesRegistry;
        }
    }
}