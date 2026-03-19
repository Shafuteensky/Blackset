namespace Blackset.DecisionInput
{
    /// <summary>
    /// Контракт для приёма ввода от UI-элементов дуэли.
    /// UI обращается напрямую к этому интерфейсу вместо статического DuelSelectionBus.
    /// </summary>
    public interface IDuelInputHandler
    {
        /// <summary>
        /// Вызывается UI при выборе дайса участником
        /// </summary>
        void OnDiceSelected(string participantId, SelectionState selection);

        /// <summary>
        /// Вызывается UI при выборе расходника участником
        /// </summary>
        void OnConsumableSelected(string participantId, SelectionState selection);

        /// <summary>
        /// Вызывается UI при запросе паса
        /// </summary>
        void OnPassRequested();
    }
}