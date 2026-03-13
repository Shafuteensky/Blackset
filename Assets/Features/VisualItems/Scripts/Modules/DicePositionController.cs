using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using UnityEngine;

namespace Blackset.Data.Items.Visual.Modules
{
    /// <summary>
    /// Управляет позицией дайса: перемещение при использовании и сброс при новой битве.
    /// </summary>
    public sealed class DicePositionController : MonoBehaviour
    {
        private string itemId;
        private Transform diceTransform;
        private Vector3 initialPosition;
        
        private DuelController duelController;
        private string ownerParticipantId;
        
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

        private void OnDiceUsed(string diceId)
        {
            // TODO: Заменить на фейк-ролл (?)
            if (diceId == itemId)
                diceTransform.position += new Vector3(0, 0, 5);
        }

        private void OnBattleStart(BattleStartEvent e)
        {
            diceTransform.position = initialPosition;
        }
    }
}