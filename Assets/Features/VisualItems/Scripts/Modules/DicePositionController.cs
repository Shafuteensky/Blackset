using System.Collections.Generic;
using System.Linq;
using Blackset.Duel.History;
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using DG.Tweening;
using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Управляет позицией дайса: перемещение при использовании и сброс при новой битве.
    /// </summary>
    public sealed class DicePositionController : MonoBehaviour
    {
        [SerializeField] private DOTweenAnimation animationTween;
        
        private string itemId;
        private Transform diceTransform;
        private Vector3 initialPosition;
        
        private DuelController duelController;
        private string ownerParticipantId;
        
        private readonly Vector3 moveDirLocal = new(-1, 0, 0);
        private readonly float moveDist = 7f;
        
        private void Start() => initialPosition = diceTransform.position;
        
        private void OnDestroy()
        {
            if (duelController == null) return;
            
            duelController.DuelContext.Participants[ownerParticipantId].FightState.onDiceUsed -= OnDiceUsed;
            duelController.EventHub.Unsubscribe<BattleStartEvent>(OnBattleStart);
        }

        /// <summary>
        /// Инициализация от координатора VisualDice.
        /// </summary>
        public void Initialize(DuelController duelController, string itemId, string ownerParticipantId, Transform diceTransform)
        {
            this.itemId = itemId;
            this.ownerParticipantId = ownerParticipantId;
            this.duelController = duelController;
            this.diceTransform = diceTransform;

            // Перемещение при использовании
            duelController.DuelContext.Participants[ownerParticipantId].FightState.onDiceUsed += OnDiceUsed;
            // Сброс при начале новой битвы
            duelController.EventHub.Subscribe<BattleStartEvent>(OnBattleStart);
        }

        private void OnDiceUsed(string diceId, bool firstTime)
        {
            if (diceId != itemId) return;
            
            // Если уже использован, то не двигается к центру
            if (!firstTime && animationTween != null)
            {
                animationTween.tween.Restart();
                return;
            }

            // Переводим направление из локального пространства родителя в мировое
            Vector3 worldDir = diceTransform.parent.TransformDirection(moveDirLocal);
            Vector3 target = diceTransform.position + worldDir * moveDist;

            diceTransform.DOMove(target, 0.5f).SetEase(Ease.OutCubic);
        }

        private void OnBattleStart(BattleStartEvent _)
        {
            diceTransform.DOMove(initialPosition, 0.5f).SetEase(Ease.OutCubic);
        }
    }
}