using Blackset.Duel.Modules;
using Blackset.Duel.Sequence;

namespace Blackset.Duel.Sequence.States
{
    /// <summary>
    /// Реестр состояний дуэли
    /// </summary>
    /// <typeparam name="TContext">Даннеы дуэли</typeparam>
    public class DuelStateRegistry<TContext> : StateRegistry<TContext>
    {
        private DuelModuleRegistry modules;
        
        /// <summary>
        /// Инициализация реестра модулей для состояний
        /// </summary>
        /// <param name="newModules">Реестр модулей</param>
        public void InitializeModules(DuelModuleRegistry newModules) => modules = newModules;
        
        public override void Add<TState>(TState state)
        {
            base.Add(state);
            
            if (state is BaseDuelState duelState && modules != null) 
                duelState.Initialize(modules);
        }
    }
}