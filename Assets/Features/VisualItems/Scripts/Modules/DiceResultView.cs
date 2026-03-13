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

        private void OnDestroy()
        {
            if (duelController == null) return;
            
            duelController.EventHub.Unsubscribe<DiceRolledEvent>(OnDiceRolled);
            duelController.EventHub.Unsubscribe<EffectsResolvedEvent>(OnEffectsResolved);
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
            duelController.EventHub.Subscribe<DiceRolledEvent>(OnDiceRolled);
            // Присвоение зарезолвенного значения результата броска
            duelController.EventHub.Subscribe<EffectsResolvedEvent>(OnEffectsResolved);
        }
        private void OnDiceRolled(DiceRolledEvent e)
        {
            if (resultText == null) return;
            if (e.ParticipantId != ownerParticipantId || e.ChosenDiceId != itemId) return;

            // TODO: Заменить на показ нужной грани
            resultText.text = e.RollResult.ToString();
        }

        private void OnEffectsResolved(EffectsResolvedEvent e)
        {
            if (resultText == null) return;

            DuelParticipantState owner = e.DuelContext.Participants[ownerParticipantId];
            resultText.text = owner.FightState.RawRollResults[itemId].ToString();
        }
    }
}