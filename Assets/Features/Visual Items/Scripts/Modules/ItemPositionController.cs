using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using DG.Tweening;
using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Управляет позицией предмета: перемещение при использовании и сброс при новой битве.
    /// </summary>
    public sealed class ItemPositionController : BaseVisualItemModule
    {
        [SerializeField] private DOTweenAnimation animationTween;
        
        private float diceMoveDist = 7.5f;
        private float consumableSelfMoveDist = 6.2f;
        private float consumableTargetMoveDist = 9.2f;

        private string itemId;
        private string ownerParticipantId;
        
        private Transform itemTransform;
        private Vector3 initialPosition;
        private readonly Vector3 moveDirLocal = new(-1, 0, 0);

        private DuelController duelController;
        bool hasMoved;
        
        private void Start() => initialPosition = itemTransform.position;

        private void OnDestroy()
        {
            if (duelController == null) return;

            Unsubscribe();
        }

        #region BaseVisualItemModule
        
        public override void Initialize(VisualItemContext context)
        {
            itemId = context.ItemId;
            ownerParticipantId = context.OwnerParticipantId;
            duelController = context.DuelController;
            itemTransform = context.Transform;

            hasMoved = false;

            Subscribe();
        }
        
        #endregion

        #region Подписка

        private void Subscribe()
        {
            duelController.EventHub.Subscribe<DiceRolledEvent>(OnDiceRolled);
            duelController.EventHub.Subscribe<ConsumableUsedEvent>(OnConsumableUsed);
            duelController.EventHub.Subscribe<BattleStartEvent>(OnBattleStart);
        }

        private void Unsubscribe()
        {
            duelController.EventHub.Unsubscribe<DiceRolledEvent>(OnDiceRolled);
            duelController.EventHub.Unsubscribe<ConsumableUsedEvent>(OnConsumableUsed);
            duelController.EventHub.Unsubscribe<BattleStartEvent>(OnBattleStart);
        }

        #endregion
        
        #region Реакция на события
        
        private void OnItemUsed(string usedItemId, string ownerId, string targetId = null)
        {
            if (usedItemId != itemId || ownerId != ownerParticipantId) return;
            if (hasMoved) return;

            hasMoved = true;

            float moveDistance = diceMoveDist;

            if (!string.IsNullOrEmpty(targetId))
            {
                moveDistance = targetId == ownerParticipantId
                    ? consumableSelfMoveDist
                    : consumableTargetMoveDist;
            }

            MoveToRollTable(GetWorldPosition(moveDistance));
        }

        private void OnDiceRolled(DiceRolledEvent handler) => OnItemUsed(handler.ChosenDiceId, handler.ParticipantId);

        private void OnConsumableUsed(ConsumableUsedEvent handler) => OnItemUsed(handler.ChosenConsumableId, 
            handler.ParticipantOwnerId, handler.ParticipantTargetId);

        private void OnBattleStart(BattleStartEvent _)
        {
            MoveToInitialPosition();
            hasMoved = false;
        }

        #endregion
        
        #region Перемещение
        
        private void MoveToInitialPosition()
        {
            itemTransform.DOMove(initialPosition, 0.5f).SetEase(Ease.OutCubic);
        }

        private void MoveToRollTable(Vector3 position)
        {
            itemTransform.DOMove(position, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                animationTween.DOComplete();
                animationTween.DOPause();
            });
            animationTween.tween.Restart();
        }

        private Vector3 GetWorldPosition(float distance)
        {
            Vector3 worldDir = itemTransform.parent.TransformDirection(moveDirLocal);
            Vector3 target = itemTransform.position + worldDir * distance;
            return target;
        }
        
        #endregion
    }
}