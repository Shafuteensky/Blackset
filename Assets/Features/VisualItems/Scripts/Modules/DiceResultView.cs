using System;
using System.Collections.Generic;
using Blackset.Duel.Participants;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Отображает результат броска дайса: сырое значение и зарезолвенное.
    /// </summary>
    public sealed class DiceResultView : MonoBehaviour
    {
        [Header("Билборд с результатом"), Space]
        [Tooltip("Корневой объект билборда (для show/hide)")]
        [SerializeField] private GameObject billboard;
        [Tooltip("Текстовое значение результата")]
        [SerializeField] private TMP_Text resultText;
        [Tooltip("Индикатор объявления")]
        [SerializeField] private GameObject declaredIndicator;

        [Header("Цифры на сторонах"), Space]
        // TODO: Сделать как в Dice Roller (пара transform-text)?
        [Tooltip("Цифры на сторонах дайса: должно быть столько же, сколько сторон объявлено в соответствующем типе дайса")]
        [SerializeField] private List<TMP_Text> sideNumbers = new List<TMP_Text>();

        private string itemId;
        private string ownerParticipantId;
        private DuelController duelController;

        private void Awake() => Reset();
        
        private void OnDestroy()
        {
            if (duelController == null) return;
            
            duelController.EventHub.Unsubscribe<DiceRolledEvent>(ShowRawResults);
            duelController.EventHub.Unsubscribe<EffectsResolvedEvent>(ShowResolvedResults);
            duelController.EventHub.Unsubscribe<BattleStartEvent>(HideResults);
            duelController.EventHub.Unsubscribe<DeclaredDiceEvent>(ShowDeclared);
            duelController.EventHub.Unsubscribe<PlanningCompletedEvent>(HideDeclaration);
        }

        /// <summary>
        /// Инициализация от координатора VisualDice.
        /// </summary>
        public void Initialize(DuelController duelController, string itemId, string ownerParticipantId)
        {
            this.itemId = itemId;
            this.ownerParticipantId = ownerParticipantId;
            this.duelController = duelController;

            // Присвоение сырого значения результата броска
            duelController.EventHub.Subscribe<DiceRolledEvent>(ShowRawResults);
            // Присвоение зарезолвенного значения результата броска
            duelController.EventHub.Subscribe<EffectsResolvedEvent>(ShowResolvedResults);
            duelController.EventHub.Subscribe<BattleStartEvent>(HideResults);
            // Состояние объявления участником
            duelController.EventHub.Subscribe<DeclaredDiceEvent>(ShowDeclared);
            duelController.EventHub.Subscribe<PlanningCompletedEvent>(HideDeclaration);
        }
        private void ShowRawResults(DiceRolledEvent handler)
        {
            if (resultText == null) return;
            // Только если этот дайс пренадлежит бросившему, и брошен был именно этот дайс
            if (handler.ParticipantId != ownerParticipantId || handler.ChosenDiceId != itemId) return;

            // TODO: Заменить на показ нужной грани
            resultText.text = handler.RollResult.ToString();
        }

        private void ShowResolvedResults(EffectsResolvedEvent _)
        {
            if (resultText == null) return;
            
            // Только если этот дайс пренадлежит бросившему
            DuelParticipantState owner = duelController.DuelContext.Participants[ownerParticipantId];
            if (owner.ParticipantId != ownerParticipantId ||
                // и брошен был именно этот дайс
                owner.FightState.TurnState.SelectedDice.Value != itemId) return;
            
            resultText.text = owner.FightState.RawRollResults[itemId].ToString();
            resultText.gameObject.SetActive(true);
        }

        private void HideResults(BattleStartEvent _)
        {
            if (resultText != null) resultText.gameObject.SetActive(false);
        }

        private void ShowDeclared(DeclaredDiceEvent handler)
        {
            if (declaredIndicator == null) return;
            
            // Только если обхявленный дайс пренадлежит бросившему
            if (handler.ParticipantOwnerId != ownerParticipantId ||
                // и объявлен был именно этот дайс
                handler.DiceId != itemId) return;
            
            declaredIndicator.SetActive(true);
        }

        private void HideDeclaration(PlanningCompletedEvent _)
        {
            if (declaredIndicator != null) declaredIndicator.SetActive(false);
        }

        private void Reset()
        {
            if (resultText != null) resultText.text = String.Empty;
            if (declaredIndicator != null) declaredIndicator.SetActive(false);
        }
    }
}