using System;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Duel.Targets;
using UnityEngine;
using UnityEngine.UI;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Контроллер показа UI-элемента ввода выборов игрока для дуэли
    /// </summary>
    // TODO Заменить панели и кнопки на подписки динамических кнопок задания выборов:
    // заспавленные фабрикой держатели id предметов, по клику оптравляют события 
    // (или просто запоминают выбор, а событие о готовности выбора по подтверждению)
    public class DuelInputPresenter : MonoBehaviour
    {
        [Header("Объявление дайса (блеф)"), Space] 
        
        [SerializeField]
        private GameObject declarationPanel;
        [SerializeField] 
        private Button confirmDeclarationButton;

        [Header("Выбор фактических действий"), Space] 
        
        [SerializeField]
        private GameObject intentPanel;
        [SerializeField] 
        private Button confirmIntentButton;
        [SerializeField] 
        private Button passButton;

        private string selectedDiceId;
        private TurnParticipantState currentIntent = new TurnParticipantState();

        /// <summary>
        /// Показать панель объявления дайса
        /// </summary>
        /// <param name="context"></param>
        /// <param name="onConfirm"></param>
        public void ShowDeclarationUI(DuelContext context, Action<string> onConfirm)
        {
            declarationPanel.SetActive(true);

            confirmDeclarationButton.onClick.RemoveAllListeners();
            confirmDeclarationButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke(selectedDiceId);
            });
        }

        /// <summary>
        /// Показать панель выбора действий
        /// </summary>
        /// <param name="context"></param>
        /// <param name="onConfirm"></param>
        public void ShowTurnIntentUI(DuelContext context, Action<TurnParticipantState> onConfirm)
        {
            intentPanel.SetActive(true);

            confirmIntentButton.onClick.RemoveAllListeners();
            confirmIntentButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke(currentIntent);
            });

            passButton.onClick.RemoveAllListeners();
            passButton.onClick.AddListener(() =>
            {
                var passIntent = new TurnParticipantState();
                passIntent.ResetForNewTurn();

                onConfirm?.Invoke(passIntent);
            });
        }

        /// <summary>
        /// Закрыть активные панели ввода
        /// </summary>
        public void Hide()
        {
            declarationPanel.SetActive(false);
            intentPanel.SetActive(false);
        }

        // TODO Эти методы вызываются кнопками выбора дайса/расходника — заменить на статические события
        
        public void SelectDice(string diceId)
        {
            selectedDiceId = diceId;
            currentIntent.ChoseDice(diceId);
        }

        public void SelectConsumable(string consumableId, ApplyTarget target)
        {
            currentIntent.ChoseConsumable(consumableId, target);
        }
    }
}