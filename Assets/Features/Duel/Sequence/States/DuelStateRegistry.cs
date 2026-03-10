using Blackset.DecisionInput;
using Blackset.Duel.Modules;
using Blackset.DuelEvents;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// Реестр состояний дуэли
    /// </summary>
    /// <typeparam name="TContext">Даннеы дуэли</typeparam>
    public class DuelStateRegistry<TContext> : StateRegistry<TContext>
    {
        private DuelModuleRegistry modules;
        private DuelInputPresenter presenter;
        private EventHub eventHub;
        
        /// <summary>
        /// Инициализация реестра модулей для состояний
        /// </summary>
        /// <param name="newModules">Реестр модулей</param>
        public void InitializeModules(DuelModuleRegistry newModules, DuelInputPresenter inputPresenter, EventHub eventHub)
        {
            modules = newModules;
            presenter = inputPresenter;
            this.eventHub = eventHub;
        }
        
        public override void Add<TState>(TState state)
        {
            base.Add(state);
            if (state is BaseDuelState duelState && modules != null) 
                duelState.Initialize(modules, presenter, eventHub);
        }
    }
}