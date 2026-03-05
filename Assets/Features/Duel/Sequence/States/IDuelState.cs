using Blackset.Duel.Modules;

namespace Blackset.Duel.Sequence
{
    /// <summary>
    /// Состояние дуэли, требующее инициализации модулями
    /// </summary>
    public interface IDuelState
    {
        /// <summary>
        /// Инициализация модулями дуэли
        /// </summary>
        /// <param name="modules">Реестр модулей дуэли</param>
        void Initialize(DuelModuleRegistry modules);
    }
}