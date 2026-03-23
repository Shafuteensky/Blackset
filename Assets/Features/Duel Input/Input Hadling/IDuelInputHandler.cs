namespace Blackset.DecisionInput
{
    /// <summary>
    /// Контракт для приёма ввода от UI-элементов дуэли.
    /// UI обращается напрямую к этому интерфейсу вместо статического DuelSelectionBus.
    /// </summary>
    public interface IDuelInputHandler
    {
        /// <summary>
        /// Вызывается в UI при выборе дайса участником
        /// </summary>
        public void OnDiceSelected(string participantId, SelectionState selection);

        /// <summary>
        /// Вызывается в UI при выборе расходника участником
        /// </summary>
        public void OnConsumableSelected(string participantId, SelectionState selection);

        /// <summary>
        /// Вызывается в UI при запросе броска выбранного дайса
        /// </summary>
        public void OnInputCompletion();
        
        /// <summary>
        /// Вызывается в UI при запросе паса
        /// </summary>
        public void OnPassRequested();
    }
}