using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Blackset.Inventories.Scripts.Items;
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
        private Transform itemTransform;
        private Vector3 initialPosition;

        private DuelController duelController;
        private string ownerParticipantId;
        private ItemClass itemClass;

        private readonly Vector3 moveDirLocal = new(-1, 0, 0);
        private readonly float moveDist = 7f;

        private void Start() => initialPosition = itemTransform.position;

        private void OnDestroy()
        {
            if (duelController == null) return;

            Unsubscribe();
            duelController.EventHub.Unsubscribe<BattleStartEvent>(OnBattleStart);
        }

        public override void Initialize(VisualItemContext context)
        {
            itemId = context.ItemId;
            ownerParticipantId = context.OwnerParticipantId;
            duelController = context.DuelController;
            itemTransform = context.Transform;
            itemClass = context.ItemClass;

            Subscribe();
            duelController.EventHub.Subscribe<BattleStartEvent>(OnBattleStart);
        }

        private void Subscribe()
        {
            var fightState = duelController.DuelContext.Participants[ownerParticipantId].FightState;

            switch (itemClass)
            {
                case ItemClass.Dice:
                    fightState.onDiceUsed += OnItemUsed;
                    break;
                case ItemClass.Consumable:
                    fightState.onConsumableUsed += OnItemUsed;
                    break;
                default:
                    Debug.LogWarning($"[ItemPositionController] Необработанный класс предмета: {itemClass}; предмет не будет перемещён");
                    break;
            }
        }

        private void Unsubscribe()
        {
            var fightState = duelController.DuelContext.Participants[ownerParticipantId].FightState;

            switch (itemClass)
            {
                case ItemClass.Dice:
                    fightState.onDiceUsed -= OnItemUsed;
                    break;
                case ItemClass.Consumable:
                    fightState.onConsumableUsed -= OnItemUsed;
                    break;
            }
        }

        private void OnItemUsed(string usedItemId, bool firstTime)
        {
            if (usedItemId != itemId) return;

            // Если уже использован, то не двигается к центру
            if (!firstTime && animationTween != null) return;

            // Переводим направление из локального пространства родителя в мировое
            Vector3 worldDir = itemTransform.parent.TransformDirection(moveDirLocal);
            Vector3 target = itemTransform.position + worldDir * moveDist;

            itemTransform.DOMove(target, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                animationTween.DOComplete();
                animationTween.DOPause();
            });
            animationTween.tween.Restart();
        }

        private void OnBattleStart(BattleStartEvent _) => MoveToInitialPosition();

        private void MoveToInitialPosition()
        {
            switch (itemClass)
            {
                case ItemClass.Dice:
                    itemTransform.DOMove(initialPosition, 0.5f).SetEase(Ease.OutCubic);
                    break;
            }
        }
    }
}