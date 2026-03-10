using System;
using Blackset.Duel.Targets;
using UnityEngine;
using UnityEngine.UI;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Отображение UI-панелей ввода выборов игрока.
    /// Не хранит игровое состояние — только показывает панели и сообщает о действиях игрока через колбэки.
    /// </summary>
    public class DuelInputPresenter : MonoBehaviour
    {
        [Header("Объявление дайса (блеф)"), Space]
        [SerializeField] private GameObject declarationPanel;
        [SerializeField] private Button confirmDeclarationButton;

        [Header("Выбор фактических действий"), Space]
        [SerializeField] private GameObject intentPanel;
        [SerializeField] private Button confirmIntentButton;
        [SerializeField] private Button passButton;

        private Action<string> onDiceSelected;
        private Action<string, ApplyTarget> onConsumableSelected;

        /// <summary>
        /// Зарегистрировать колбэки выбора предметов.
        /// Вызывается один раз при инициализации источника решений.
        /// </summary>
        public void SetItemCallbacks(Action<string> onDice, Action<string, ApplyTarget> onConsumable)
        {
            onDiceSelected = onDice;
            onConsumableSelected = onConsumable;
        }

        /// <summary> Показать панель объявления дайса </summary>
        /// <param name="onConfirm">Игрок подтвердил объявление</param>
        public void ShowDeclarationUI(Action onConfirm)
        {
            declarationPanel.SetActive(true);

            confirmDeclarationButton.onClick.RemoveAllListeners();
            confirmDeclarationButton.onClick.AddListener(() => onConfirm?.Invoke());
        }

        /// <summary> Показать панель выбора действий </summary>
        /// <param name="onConfirm">Игрок подтвердил намерения</param>
        /// <param name="onPass">Игрок спасовал</param>
        public void ShowTurnIntentUI(Action onConfirm, Action onPass)
        {
            intentPanel.SetActive(true);

            confirmIntentButton.onClick.RemoveAllListeners();
            confirmIntentButton.onClick.AddListener(() => onConfirm?.Invoke());

            passButton.onClick.RemoveAllListeners();
            passButton.onClick.AddListener(() => onPass?.Invoke());
        }

        /// <summary> Закрыть активные панели ввода </summary>
        public void Hide()
        {
            declarationPanel.SetActive(false);
            intentPanel.SetActive(false);
        }

        // Вызываются кнопками выбора дайса/расходника (спавнятся фабрикой)

        public void NotifyDiceSelected(string diceId) => onDiceSelected?.Invoke(diceId);

        public void NotifyConsumableSelected(string consumableId, ApplyTarget target) =>
            onConsumableSelected?.Invoke(consumableId, target);
    }
}