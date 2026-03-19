using Blackset.Duel;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Данные о выборе и цели применения дайса/расходника
    /// </summary>
    public class SelectionState
    {
        /// <summary>
        /// Идентификатор выбранного предмета из сборки
        /// </summary>
        public string SelectedItemId { get; private set; }
        /// <summary>
        /// Идентификатор участника-цели
        /// </summary>
        public string TargetParticipantId { get; private set; }
        /// <summary>
        /// Идентификатор дайса-цели
        /// </summary>
        public string TargetDiceId { get; private set; }
        
        /// <summary>
        /// Выбран ли дайс/расходник
        /// </summary>
        public bool IsItemSelected => !string.IsNullOrEmpty(SelectedItemId);
        /// <summary>
        /// Выбран ли участник-цель
        /// </summary>
        public bool IsTargetParticipantSelected => !string.IsNullOrEmpty(TargetParticipantId);
        /// <summary>
        /// Выбран ли дайс-цель
        /// </summary>
        public bool IsTargetDiceSelected => !string.IsNullOrEmpty(TargetDiceId);

        /// <summary>
        /// Новые данные о выборе и цели применения дайса/расходника
        /// </summary>
        public SelectionState()
        {
            SelectedItemId = string.Empty;
            TargetParticipantId = string.Empty;
            TargetDiceId = string.Empty;
        }
        
        /// <summary>
        /// Новые данные о выборе и цели применения дайса/расходника
        /// </summary>
        /// <param name="selectedItemId">Идентификатор выбранного предмета из сборки</param>
        /// <param name="targetParticipantId">Идентификатор участника-цели</param>
        /// <param name="targetDiceId">Идентификатор дайса-цели</param>
        public SelectionState(string selectedItemId, string targetParticipantId = null, string targetDiceId = null)
        {
            SelectedItemId = selectedItemId;
            TargetParticipantId = targetParticipantId;
            TargetDiceId = targetDiceId;
        }

        /// <summary>
        /// Выбрать дайс/расходник
        /// </summary>
        /// <param name="itemId">Идентификатор предмета</param>
        public void SelectItem(string itemId) => SelectedItemId = itemId;
        /// <summary>
        /// Выбрать участника-цель
        /// </summary>
        /// <param name="itemId">Идентификатор участника-цель</param>
        public void SelectTargetParticipant(string participantId) => TargetParticipantId = participantId;
        /// <summary>
        /// Выбрать дайс-цель
        /// </summary>
        /// <param name="itemId">Идентификатор дайс-цель</param>
        public void SelectTargetDice(string targetDiceId) => TargetDiceId = targetDiceId;

        /// <summary>
        /// Получить данные об использовании дайса/расходника
        /// </summary>
        public ItemUseContext ToUseContext()
        {
            ItemUseContext itemUseContext = new ItemUseContext
            (
                SelectedItemId,
                TargetParticipantId,
                TargetDiceId
            );
            return itemUseContext;
        }
    }
}