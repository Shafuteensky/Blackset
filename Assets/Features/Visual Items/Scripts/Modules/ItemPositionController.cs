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

        private string itemId;
        private string ownerParticipantId;
        
        private Transform itemTransform;
        private Vector3 initialPosition;
        private readonly Vector3 moveDirLocal = new(-1, 0, 0);
        private readonly float moveDist = 7f;

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
        
        private void OnItemUsed(string usedItemId, string ownerId)
        {
            if (usedItemId != itemId || ownerId != ownerParticipantId) return;
            if (hasMoved && animationTween != null) return;
            hasMoved = true;

            MoveToRollTable(GetWorldPosition());
        }

        private void OnDiceRolled(DiceRolledEvent handler) => OnItemUsed(handler.ChosenDiceId, handler.ParticipantId);

        private void OnConsumableUsed(ConsumableUsedEvent handler) => OnItemUsed(handler.ChosenConsumableId, handler.ParticipantId);

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

        private Vector3 GetWorldPosition()
        {
            Vector3 worldDir = itemTransform.parent.TransformDirection(moveDirLocal);
            Vector3 target = itemTransform.position + worldDir * moveDist;
            return target;
        }
        
        #endregion
    }
}