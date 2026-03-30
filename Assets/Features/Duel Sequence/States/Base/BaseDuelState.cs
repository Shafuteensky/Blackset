using Blackset.Data.Registries;
using Blackset.Duel.Modules;
using Extensions.Events;
using Extensions.Log;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// Базовое состояние дуэли со входным параметром
    /// </summary>
    public class BaseDuelState
    {
        protected DuelModuleRegistry modules;
        protected EventHub eventHub;
        protected readonly GameData gameData = GameData.Instance;
        
        /// <summary>
        /// Инициализация данных состояния
        /// </summary>
        /// <param name="modules">Реестр модулей обработки данных дуэли</param>
        public void Initialize(DuelModuleRegistry modulesRegistry,  EventHub duelEventHub)
        {
            ServiceGuard.NotNull(modulesRegistry, nameof(modulesRegistry));
            ServiceGuard.NotNull(duelEventHub, nameof(duelEventHub));
            
            modules = modulesRegistry;
            eventHub = duelEventHub;
        }
    }
}