using Blackset.Duel.Sequence;
using Blackset.DuelEvents.EventTypes;
using UnityEngine;
using System.Linq;
using System.Text;
using Blackset.Data.Items.Types;
using Blackset.Duel.Context;
using Blackset.Duel.Participants;
using Blackset.Inventories.Cells;
using Blackset.Inventories.Items;
using Extensions.Log;

namespace Blackset.GameDebug
{
    /// <summary>
    /// Дебак состояния дуэли
    /// </summary>
    public class DuelStateDebug : MonoBehaviour
    {
        [SerializeField] private bool enableLogs = true;
        [SerializeField] DuelController duelController;

        public void OnEnable()
        {
            duelController.EventHub.Subscribe<BattleStartEvent>(Log);
            duelController.EventHub.Subscribe<BattleEndEvent>(Log);
        }

        public void OnDisable()
        {
            duelController.EventHub.Unsubscribe<BattleStartEvent>(Log);
            duelController.EventHub.Unsubscribe<BattleEndEvent>(Log);
        }

        public void Log(BattleStartEvent handle)
        {
            if (handle.DuelContext.Progress.FightNumber.Value == 1)
                Log(handle.DuelContext);
        }
        public void Log(BattleEndEvent handle) => Log(handle.DuelContext);
        
        /// <summary>
        /// Полный дамп состояния дуэли — вызывай по колбеку когда нужно
        /// </summary>
        public void Log(DuelContext context)
        {
            if (!enableLogs) return;
            
            if (context == null)
            {
                ServiceDebug.LogError("DuelContext is null — нечего выводить");
                return;
            }

            LogGeneral(context);
            LogProgress(context);
            LogTargetValue(context);
            LogRules(context);

            foreach (var participant in context.Participants.Values)
                LogParticipant(context, participant);
        }

        #region Секции

        /// <summary>
        /// Общие данные дуэли
        /// </summary>
        private void LogGeneral(DuelContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Общие данные —————————————————");
            sb.AppendLine($"  Сид:         {(string.IsNullOrEmpty(context.Seed) ? "—" : context.Seed)}");
            sb.AppendLine($"  Игрок id:    {(string.IsNullOrEmpty(context.PlayerId) ? "—" : context.PlayerId)}");
            sb.AppendLine($"  Оппонент id: {(string.IsNullOrEmpty(context.OpponentId) ? "—" : context.OpponentId)}");
            sb.AppendLine($"  Контракт:    {(context.Contract != null ? context.Contract.ToString() : "—")}");

            bool stormActive = context.Storm != null && context.Storm.IsActive();
            if (stormActive)
                sb.Append($"  Шторм:       активен ({context.Storm})");
            else
                sb.Append($"  Шторм:       не активен");

            ServiceDebug.Log(sb.ToString());
        }

        /// <summary>
        /// Прогресс дуэли
        /// </summary>
        private void LogProgress(DuelContext context)
        {
            var p = context.Progress;
            var sb = new StringBuilder();
            sb.AppendLine($"Прогресс");
            sb.AppendLine($"  Текущий бросок:   {p.ThrowNumber.Value}");
            sb.AppendLine($"  Текущий бой:      {p.FightNumber.Value}");
            sb.Append(    $"  Максимум боев:    {context.Rules.MaxFightsPerDuel}");

            ServiceDebug.Log(sb.ToString());
        }

        /// <summary>
        /// Целевое значение
        /// </summary>
        private void LogTargetValue(DuelContext context)
        {
            var t = context.TargetValue;
            var sb = new StringBuilder();
            sb.AppendLine($"Целевое значение");
            sb.Append(    $"  ЦЗ: {t.TargetValue.Value}");

            ServiceDebug.Log(sb.ToString());
        }

        /// <summary>
        /// Активные правила
        /// </summary>
        private void LogRules(DuelContext context)
        {
            var r = context.Rules;
            var sb = new StringBuilder();
            sb.AppendLine($"Правила");
            sb.AppendLine($"  Максимум бросков за бой: {r.MaxThrowsPerFight}");
            sb.AppendLine($"  Максимум боев в дуэли:   {r.MaxFightsPerDuel}");
            sb.AppendLine($"  Политика победы:         {r.FightWinPolicy}");
            sb.Append(    $"  Политика поражения:      {r.FightLossPolicy}");

            ServiceDebug.Log(sb.ToString());
        }

        /// <summary>
        /// Полное состояние участника
        /// </summary>
        private void LogParticipant(DuelContext context, DuelParticipantState participant)
        {
            var fight = participant.FightState;
            var turn = fight.TurnState;

            // Определяем наличие потенциальных проблем
            bool hasWarnings = fight.HasGivenUp.Value || turn.HasPassed.Value;

            var sb = new StringBuilder();
            sb.AppendLine($"Участник: id='{participant.ParticipantId}' ({(participant.IsPlayer ? "Игрок" : "Бот")})");

            // --- Дуэль ---
            sb.AppendLine($"  [Дуэль]");
            sb.AppendLine($"    Побед в боях:   {participant.FightsWon.Value}");
            sb.AppendLine($"    Пулы готовы:    {participant.IsPoolsInited}");
            sb.AppendLine($"    Сборки готовы:  {participant.IsSetsInited}");
            if (!participant.IsPlayer)
            {
                sb.AppendLine($"    Уровень доверия: {participant.TrustLevel.Value:F2}");
                sb.AppendLine($"    Уровень паники:  {participant.PanicLevel.Value:F2}");
            }
            
            sb.AppendLine($"  [Дайсы в сборке]");
            foreach (InventoryCell cell in participant.Sets.DiceSetInventory.Data)
            {
                InventoryItem item = participant.Sets.DiceSetInventory.GetCellItemData(cell);
                InventoryItemType itemType = participant.Sets.DiceSetInventory.GetCellTypeData(cell);
                if (item == null) continue;
                sb.AppendLine($"    {item.DataName}, {itemType.DataName}");
            }
            
            sb.AppendLine($"  [Расходники в сборке]");
            foreach (InventoryCell cell in participant.Sets.ConsumableSetInventory.Data)
            {
                InventoryItem item = participant.Sets.DiceSetInventory.GetCellItemData(cell);
                InventoryItemType itemType = participant.Sets.DiceSetInventory.GetCellTypeData(cell);
                if (item == null) continue;
                sb.AppendLine($"    {item.DataName}, {itemType.DataName}");
            }
            
            // --- Текущий бой ---
            sb.AppendLine($"  [Бой]");
            sb.AppendLine($"    Счёт:           {fight.FightScore.Value}");
            sb.AppendLine($"    Бросков:        {fight.Throws.Value}");
            sb.AppendLine($"    Сдался:         {fight.HasGivenUp.Value}");

            string dicesUsed = fight.DicesUsed.Count > 0
                ? string.Join(", ", fight.DicesUsed)
                : "—";
            sb.AppendLine($"    Дайсы за бой:   [{dicesUsed}]");

            string consumablesUsed = fight.ConsumablesUsed.Count > 0
                ? string.Join(", ", fight.ConsumablesUsed)
                : "—";
            sb.AppendLine($"    Расходники:     [{consumablesUsed}]");

            string rawRolls = fight.RawRollResults.Count > 0
                ? string.Join(", ", fight.RawRollResults.Select(kv => $"{kv.Key}:{kv.Value}"))
                : "—";
            sb.AppendLine($"    Сырые роллы:    [{rawRolls}]");

            // --- Текущий ход ---
            sb.AppendLine($"  [Ход]");
            sb.AppendLine($"    Может действовать:   {turn.CanAct}");
            sb.AppendLine($"    Все действия готовы: {turn.AllActionsDone}");
            sb.AppendLine($"    Спасовал:            {turn.HasPassed.Value}");

            string declared = string.IsNullOrEmpty(turn.DeclaredDice.Value) ? "—" : turn.DeclaredDice.Value;
            sb.AppendLine($"    Объявленный дайс:    {declared}");

            string chosen = string.IsNullOrEmpty(turn.SelectedDice.Value) ? "—" : turn.SelectedDice.Value;
            sb.AppendLine($"    Выбранный дайс:      {chosen}");

            string consumable = string.IsNullOrEmpty(turn.SelectedConsumable.Value)
                ? "—"
                : $"{turn.SelectedConsumable.Value} → {turn.SelectedConsumable.Value}";
            sb.Append(    $"    Расходник в ходе:    {consumable}");

            // Знания об участнике
            if (context.Knowledge.TryGetValue(participant.ParticipantId, out var knowledge))
            {
                sb.AppendLine();
                sb.AppendLine($"  [Знания]");
                sb.Append($"    {knowledge}");
            }

            if (hasWarnings)
                ServiceDebug.LogWarning(sb.ToString());
            else
                ServiceDebug.Log(sb.ToString());
        }

        #endregion
    }
}
