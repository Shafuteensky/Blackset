using System.Collections.Generic;
using Blackset.Duel.Participants;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Blackset.Inventories;
using Extensions.Log;
using TMPro;
using UnityEngine;

namespace Blackset.Data.Items.Visual
{
    /// <summary>
    /// Визуальный префаб дайса
    /// </summary>
    public sealed class VisualDice : BaseVisual
    {
        // TODO Заменить на показ нужной грани
        [Header("Элемент текста с результатом броска"), Space]
        [Tooltip("Билборд с результатом")]
        [SerializeField] private GameObject billboard;
        [Tooltip("Текстовое значение")]
        [SerializeField] private TMP_Text resultText;
        
        [Header("Цифры на сторонах"), Space]
        // TODO Сделать как в Dice Roller (пара transform-text)?
        [Tooltip("Цифры на сторонах дайса: должно быть столько же, сколько и сторон объявленно в соответствющем типе дайса")]
        [SerializeField] private List<TMP_Text> sideNumbers = new List<TMP_Text>();
        
        [Header("Визуал"), Space]
        [SerializeField] MeshFilter meshFilter;
        [SerializeField] MeshRenderer meshRenderer;

        private Vector3 initialPosition;
        
        public override void Initialize(string itemId, string ownerParticipantId,
            Inventory inventory, string newItemCellId)
        {
            base.Initialize(itemId, ownerParticipantId, inventory, newItemCellId);
            
            DuelController duelController = DuelController.Instance;
            if (duelController == null)
            {
                ServiceDebug.LogError($"Не найден инстанс {nameof(DuelController)}, визуальный дайс не инициализирован");
                return;
            }
            
            initialPosition = transform.position;
            
            // Перемещение при использовании
            duelController.DuelContext.Participants[ownerParticipantId].FightState.onDiceUsed += MoveDice;
            // Сброс при начале новой битвы
            duelController.EventHub.Subscribe<BattleStartEvent>(ResetDice);
            // Присвоение сырого значения результата броска
            duelController.EventHub.Subscribe<DiceRolledEvent>(UpdateResultText_OnRoll);
            // Присвоение зарезолвенного значения результата броска
            duelController.EventHub.Subscribe<EffectsResolvedEvent>(UpdateResultText_OnResolve);
        }

        #region Положение
        
        private void MoveDice(string diceId)
        {
            // TODO Заменить на фейк-ролл (?)
            if (diceId == ItemId) 
                gameObject.transform.position += new Vector3(0, 0, 5);
        }

        private void ResetDice(BattleStartEvent handler)
        {
            transform.position = initialPosition;
        }
        
        #endregion

        #region Результат ролла
        
        private void UpdateResultText_OnRoll(DiceRolledEvent handler)
        {
            if (resultText == null) return;
            
            if (handler.ParticipantId == OwnerParticipantId && handler.ChosenDiceId == ItemId) 
                resultText.text = handler.RollResult.ToString();
        }

        private void UpdateResultText_OnResolve(EffectsResolvedEvent handler)
        {
            if (resultText == null) return;

            DuelParticipantState owner = handler.DuelContext.Participants[OwnerParticipantId];
            resultText.text = owner.FightState.RawRollResults[ItemId].ToString();
        }
        
        #endregion
    }
}