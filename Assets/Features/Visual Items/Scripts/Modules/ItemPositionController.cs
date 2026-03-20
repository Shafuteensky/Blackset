// ItemPositionController.cs
using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using Blackset.Inventories.Scripts.Items;
using DG.Tweening;
using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    public sealed class ItemPositionController : MonoBehaviour
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

        public void Initialize(DuelController duelController, string itemId, string ownerParticipantId, Transform itemTransform, ItemClass itemClass)
        {
            this.itemId = itemId;
            this.ownerParticipantId = ownerParticipantId;
            this.duelController = duelController;
            this.itemTransform = itemTransform;
            this.itemClass = itemClass;

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
                    Debug.LogWarning($"[ItemPositionController] Необработанный класс предмета: {itemClass}; предмет не будет перемещен");
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

        private void OnItemUsed(string usedItemId) => OnItemUsed(usedItemId, false);
        
        private void OnItemUsed(string usedItemId, bool firstTime)
        {
            if (usedItemId != itemId) return;

            if (!firstTime && animationTween != null) return;

            Vector3 worldDir = itemTransform.parent.TransformDirection(moveDirLocal);
            Vector3 target = itemTransform.position + worldDir * moveDist;

            itemTransform.DOMove(target, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                animationTween.DOComplete();
                animationTween.DOPause();
            });
            animationTween.tween.Restart();
        }

        private void OnBattleStart(BattleStartEvent _)
        {
            itemTransform.DOMove(initialPosition, 0.5f).SetEase(Ease.OutCubic);
        }
    }
}