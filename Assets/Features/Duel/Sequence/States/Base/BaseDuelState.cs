using Blackset.DecisionInput;
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
        protected DuelInputPresenter presenter;
        
        /// <summary>
        /// Инициализация данных состояния
        /// </summary>
        /// <param name="modules">Реестр модулей обработки данных дуэли</param>
        public void Initialize(DuelModuleRegistry modulesRegistry,  DuelInputPresenter inputPresenter)
        {
            ServiceGuard.NotNull(modulesRegistry, nameof(modulesRegistry));
            ServiceGuard.NotNull(inputPresenter, nameof(inputPresenter));
            
            modules = modulesRegistry;
            presenter = inputPresenter;
        }
    }
}