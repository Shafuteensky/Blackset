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
        private Action<string> onDiceSelected;
        private Action<string, ApplyTarget> onConsumableSelected;
        private Action onPassRequested;

        private InputMode currentMode;
        private string activeParticipantId;

        protected virtual void OnEnable()
        {
            DuelSelectionBus.DiceSelected += HandleDiceSelected;
            DuelSelectionBus.ConsumableSelected += HandleConsumableSelected;
        }

        protected virtual void OnDisable()
        {
            DuelSelectionBus.DiceSelected -= HandleDiceSelected;
            DuelSelectionBus.ConsumableSelected -= HandleConsumableSelected;
        }

        /// <summary>
        /// Зарегистрировать обработчики выбора
        /// </summary>
        public void SetItemCallbacks(
            Action<string> newOnDiceSelected,
            Action<string, ApplyTarget> newOnConsumableSelected,
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
        public void BeginDeclaration(string participantId)
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
            if (currentMode == InputMode.Intent) onPassRequested?.Invoke();
        }

        #region Отправка запросов
        
        private void HandleDiceSelected(string ownerParticipantId, string diceId)
        {
            if (CanAccept(ownerParticipantId)) onDiceSelected?.Invoke(diceId);
        }

        private void HandleConsumableSelected(
            string ownerParticipantId,
            string consumableId,
            ApplyTarget target)
        {
            if (!CanAccept(ownerParticipantId) || currentMode != InputMode.Intent) return;
            onConsumableSelected?.Invoke(consumableId, target);
        }
        
        #endregion

        private bool CanAccept(string ownerParticipantId)
        {
            if (currentMode == InputMode.None || string.IsNullOrEmpty(activeParticipantId)) return false;
            return activeParticipantId == ownerParticipantId;
        }
    }
}