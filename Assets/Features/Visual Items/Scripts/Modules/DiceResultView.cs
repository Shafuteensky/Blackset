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
    /// Отображение результата броска дайса (сырое значение и зарезолвенное)
    /// </summary>
    public sealed class DiceResultView : BaseVisualItemModule
    {
        [Header("Билборд с результатом"), Space]
        [Tooltip("Корневой объект билборда (для show/hide)")]
        [SerializeField] private GameObject billboard;
        [Tooltip("Текстовое значение финального результата")]
        [SerializeField] private TMP_Text finalResultText;
        [Tooltip("Текстовое значение сырого результата")]
        [SerializeField] private TMP_Text rawResultText;

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

        public override void Initialize(VisualItemContext context)
        {
            itemId = context.ItemId;
            ownerParticipantId = context.OwnerParticipantId;
            duelController = context.DuelController;

            // Присвоение сырого значения результата броска
            duelController.EventHub.Subscribe<DiceRolledEvent>(ShowRawResults);
            // Присвоение зарезолвенного значения результата броска
            duelController.EventHub.Subscribe<EffectsResolvedEvent>(ShowResolvedResults);
            duelController.EventHub.Subscribe<BattleStartEvent>(HideResults);
        }

        private void ShowRawResults(DiceRolledEvent handler)
        {
            // Только если этот дайс принадлежит бросившему, и брошен был именно этот дайс
            if (handler.ParticipantId != ownerParticipantId || handler.ChosenDiceId != itemId) return;

            DuelParticipantState owner = duelController.DuelContext.Participants[ownerParticipantId];
            string rawResult = owner.FightState.RollHistory[itemId].RawResult.ToString();
            
            // TODO: Заменить на показ нужной грани
            if (finalResultText != null) finalResultText.text = rawResult;
            if (rawResultText != null) 
            {
                EnableRawResult(false);
                rawResultText.text = rawResult;
            }
        }

        private void ShowResolvedResults(EffectsResolvedEvent _)
        {
            if (finalResultText == null) return;

            // Только если этот дайс принадлежит бросившему
            DuelParticipantState owner = duelController.DuelContext.Participants[ownerParticipantId];
            if (owner.ParticipantId != ownerParticipantId ||
                // и брошен был именно этот дайс
                owner.FightState.TurnState.SelectedDice.Value != itemId) return;

            int finalResult = owner.FightState.RollHistory[itemId].FinalResult;
            int rawResult = owner.FightState.RollHistory[itemId].RawResult;
            
            finalResultText.text = finalResult.ToString();
            EnableFinalResult(true);

            if (finalResult != rawResult) EnableRawResult(true);
        }

        private void HideResults(BattleStartEvent _)
        {
            EnableRawResult(false);
            EnableFinalResult(false);
        }

        private void Reset()
        {
            if (finalResultText != null) finalResultText.text = String.Empty;
            if (rawResultText != null) rawResultText.text = String.Empty;
        }

        private void EnableRawResult(bool isEnabled)
        {
            if (rawResultText != null) rawResultText.gameObject.SetActive(isEnabled);
        }

        private void EnableFinalResult(bool isEnabled)
        {
            if (finalResultText != null) finalResultText.gameObject.SetActive(isEnabled);
        }
    }
}