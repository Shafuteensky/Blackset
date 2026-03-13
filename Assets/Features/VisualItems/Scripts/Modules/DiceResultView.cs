using System;
using System.Collections.Generic;
using Blackset.Duel.Participants;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using TMPro;
using UnityEngine;

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
            // Очистка результата
            duelController.EventHub.Subscribe<BattleStartEvent>(HideResults);
        }
        private void ShowRawResults(DiceRolledEvent handler)
        {
            if (resultText == null) return;
            // Только если этот дайс пренадлежит бросившему, и брошен был именно этот дайс
            if (handler.ParticipantId != ownerParticipantId || handler.ChosenDiceId != itemId) return;

            // TODO: Заменить на показ нужной грани
            resultText.text = handler.RollResult.ToString();
        }

        private void ShowResolvedResults(EffectsResolvedEvent handler)
        {
            if (resultText == null) return;
            
            // Только если этот дайс пренадлежит бросившему
            DuelParticipantState owner = handler.DuelContext.Participants[ownerParticipantId];
            if (owner.ParticipantId != ownerParticipantId ||
                // и брошен был именно этот дайс
                owner.FightState.TurnState.ChosenDice.Value != itemId) return;
            
            resultText.text = owner.FightState.RawRollResults[itemId].ToString();
        }

        private void HideResults(BattleStartEvent _) => Reset();

        private void Reset()
        {
            if (resultText == null) return;
            
            resultText.text = String.Empty;
        }
    }
}