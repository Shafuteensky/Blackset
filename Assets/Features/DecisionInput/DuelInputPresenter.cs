using System;
using Blackset.Duel.Targets;
using UnityEngine;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Управляет доступностью ввода в фазах дуэли.
    /// Сам UI не показывает, только принимает или игнорирует выборы.
    /// </summary>
    public class DuelInputPresenter : MonoBehaviour
    {
        private Action<SelectionState> onDiceSelected;
        private Action<SelectionState> onConsumableSelected;
        private Action onPassRequested;

        private InputMode currentMode;
        private string activeParticipantId;

        protected virtual void OnEnable()
        {
            DuelSelectionBus.DiceSelected += HandleDiceSelected;
            DuelSelectionBus.ConsumableSelected += HandleConsumableSelected;
            DuelSelectionBus.PassRequested += HandlePassRequested;
        }

        protected virtual void OnDisable()
        {
            DuelSelectionBus.DiceSelected -= HandleDiceSelected;
            DuelSelectionBus.ConsumableSelected -= HandleConsumableSelected;
            DuelSelectionBus.PassRequested -= HandlePassRequested;
        }

        /// <summary>
        /// Зарегистрировать обработчики выбора
        /// </summary>
        public void SetItemCallbacks(
            Action<SelectionState> newOnDiceSelected,
            Action<SelectionState> newOnConsumableSelected,
            Action newOnPassRequested)
        {
            onDiceSelected = newOnDiceSelected;
            onConsumableSelected = newOnConsumableSelected;
            onPassRequested = newOnPassRequested;
        }

        #region Фазы ввода
        
        /// <summary>
        /// Разрешить объявление дайса
        /// </summary>
        public void BeginSelectionInput(string participantId)
        {
            activeParticipantId = participantId;
            currentMode = InputMode.Declaration;
        }

        /// <summary>
        /// Разрешить выбор фактического действия
        /// </summary>
        public void BeginIntentSelection(string participantId)
        {
            activeParticipantId = participantId;
            currentMode = InputMode.Intent;
        }
        
        #endregion

        /// <summary>
        /// Запретить ввод
        /// </summary>
        public void EndInput()
        {
            activeParticipantId = string.Empty;
            currentMode = InputMode.None;
        }

        /// <summary>
        /// Явный запрос паса извне
        /// </summary>
        public void RequestPass()
        {
            if (currentMode != InputMode.Intent) onPassRequested?.Invoke();
        }

        #region Отправка запросов
        
        private void HandleDiceSelected(string ownerParticipantId, SelectionState selection)
        {
            if (!CanAccept(ownerParticipantId)) return;
            
            onDiceSelected?.Invoke(selection);
        }

        private void HandleConsumableSelected(string ownerParticipantId, SelectionState selection)
        {
            if (!CanAccept(ownerParticipantId) || currentMode != InputMode.Intent) return;
            
            onConsumableSelected?.Invoke(selection);
        }
        
        private void HandlePassRequested()
        {
            if (currentMode != InputMode.None) onPassRequested?.Invoke();
        }
        
        #endregion

        private bool CanAccept(string ownerParticipantId)
        {
            if (currentMode == InputMode.None || string.IsNullOrEmpty(activeParticipantId)) 
                return false;
            else 
                return activeParticipantId == ownerParticipantId;
        }
    }
}