namespace Blackset.DecisionInput
{
    /// <summary>
    /// Данные о выборе и цели применения дайса/расходника
    /// </summary>
    public class SelectionState
    {
        #region Данные о выборе
        
        /// <summary>
        /// Идентификатор выбранного предмета из сборки
        /// </summary>
        public string SelectedItemId { get; private set; }
        
        /// <summary>
        /// Выбран ли дайс/расходник
        /// </summary>
        public bool IsItemSelected => !string.IsNullOrEmpty(SelectedItemId);

        #endregion
        
        #region Конструкторы
        
        /// <summary>
        /// Новые данные о выборе и цели применения дайса/расходника
        /// </summary>
        public SelectionState() => SelectedItemId = string.Empty;
        
        /// <summary>
        /// Новые данные о выборе и цели применения дайса/расходника
        /// </summary>
        /// <param name="selectedItemId">Идентификатор выбранного предмета из сборки</param>
        public SelectionState(string selectedItemId) => SelectedItemId = selectedItemId;
        
        #endregion

        #region Назначение выбора
        
        /// <summary>
        /// Выбрать дайс/расходник
        /// </summary>
        /// <param name="itemId">Идентификатор предмета</param>
        public void SelectItem(string itemId) => SelectedItemId = itemId;
        
        #endregion
    }
}