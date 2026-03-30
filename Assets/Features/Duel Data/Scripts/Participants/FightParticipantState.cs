using System;
using System.Collections.Generic;
using System.Linq;
using Blackset.Duel.Rolls;
using Blackset.Effects;
using Extensions.Helpers;
using Extensions.Reactive;
using UnityEngine;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Состояние участника дуэли на текущую битву
    /// </summary>
    public class FightParticipantState
    {
        #region События
        /// <summary>
        /// Дайс из сборки использован в текущем бою [идентификатор_дайса_в_сборке, первое_использование]
        /// </summary>
        public event Action<string, bool> onDiceUsed; 
        /// <summary>
        /// Расходник из сборки использован в текущем бою
        /// </summary>
        public event Action<string, bool> onConsumableUsed; 
        #endregion

        #region Геттеры
        /// <summary>
        /// Результаты бросков дайсов (без эффектов и прочего — "сырые") [id_дайса_в_сборке, результат]
        /// </summary>
        public Dictionary<string, int> RawRollResults => rollHistory.Values.ToDictionary(x => x.DiceInstanceId, x => x.RawResult);
        /// <summary>
        /// История бросков участника в текущем бою
        /// </summary>
        public Dictionary<string, RollHistoryEntry> RollHistory => rollHistory.ToDictionary();
        /// <summary>
        /// Состояние на текущий ход
        /// </summary>
        public TurnParticipantState TurnState => turnState;
        #endregion
        
        #region Автосвойства
        /// <summary>
        /// Сдался в текущем бою
        /// </summary>
        public ReactiveProperty<bool> HasGivenUp { get; private set; } = new(false);
        /// <summary>
        /// Количество совершенных бросков
        /// </summary>
        public ReactiveProperty<int> Throws { get; private set; } = new(0);
        /// <summary>
        /// Счет боя 
        /// </summary>
        public ReactiveProperty<int> FightScore { get; private set; } = new(0);
        /// <summary>
        /// Постоянный модификаторы счета боя
        /// </summary>
        public ReactiveProperty<int> PersistentFightScoreModifier { get; private set; } = new(0);
        #endregion

        #region Внутренние данные
        private readonly OrderedDictionary<string, RollHistoryEntry> rollHistory = new();
        private readonly OrderedDictionary<string, ItemUsageState> diceUsageStates = new();
        private readonly OrderedDictionary<string, ItemUsageState> consumableUsageStates = new();
        private readonly TurnParticipantState turnState = new();
        #endregion

        #region Конструктор
        /// <summary>
        /// Создание хранилища данных о состоянии участника дуэли во время битвы
        /// </summary>
        public FightParticipantState() => ResetForNewFight();
        #endregion
        
        #region Получение данных

        /// <summary>
        /// Получить список идентификаторов использованных за бой дайсов
        /// </summary>
        public List<string> GetUsedDices() => 
            diceUsageStates.ToDictionary().Where(x => x.Value.IsUsed).Select(x => x.Key).ToList();

        /// <summary>
        /// Получить список идентификаторов использованных за бой расходников
        /// </summary>
        public List<string> GetUsedConsumables() => 
            consumableUsageStates.ToDictionary().Where(x => x.Value.IsUsed).Select(x => x.Key).ToList();

        /// <summary>
        /// Использован ли расходник за этот бой
        /// </summary>
        /// <param name="consumable">Идентификатор проверяемого расходника</param>
        /// <returns>true если был использован хоть раз, иначе false</returns>
        public bool IsConsumableUsed(string consumable) => GetUsedConsumables().Contains(consumable);

        /// <summary>
        /// Попробовать получить запись истории последнего броска
        /// </summary>
        public bool TryGetLastRoll(out RollHistoryEntry rollEntry)
        {
            if (rollHistory.Count == 0)
            {
                rollEntry = null;
                return false;
            }

            return rollHistory.TryGetLast(out rollEntry);
        }

        /// <summary>
        /// Попробовать получить запись истории последнего броска
        /// </summary>
        public bool TryGetPreviousRoll(out RollHistoryEntry rollEntry)
        {
            if (rollHistory.Count == 0)
            {
                rollEntry = null;
                return false;
            }

            return rollHistory.TryGetPrevious(out rollEntry);
        }
        
        #endregion
        
        #region Состояния использования предметов
        
        /// <summary>
        /// Получить или создать состояние применения дайса
        /// </summary>
        public ItemUsageState GetOrCreateDiceUsageState(string diceId)
        {
            if (!diceUsageStates.TryGetValue(diceId, out ItemUsageState usageState))
            {
                usageState = new ItemUsageState(diceId, EffectSourceKind.Dice);
                diceUsageStates.Add(diceId, usageState);
            }

            return usageState;
        }

        /// <summary>
        /// Получить или создать состояние применения расходника
        /// </summary>
        public ItemUsageState GetOrCreateConsumableUsageState(string consumableId)
        {
            if (!consumableUsageStates.TryGetValue(consumableId, out ItemUsageState usageState))
            {
                usageState = new ItemUsageState(consumableId, EffectSourceKind.Consumable);
                consumableUsageStates.Add(consumableId, usageState);
            }

            return usageState;
        }
        
        #endregion
        
        #region Сброс данных
        
        /// <summary>
        /// Сброс данных до изначальных для нового боя
        /// </summary>
        public void ResetForNewFight()
        {
            HasGivenUp.Value = false;

            Throws.Value = 0;
            FightScore.Value = 0;
            PersistentFightScoreModifier.Value = 0;

            rollHistory.Clear();
            diceUsageStates.Clear();
            consumableUsageStates.Clear();
            
            turnState.ResetForNewTurn();
            turnState.InitFightState(this);
        }
        
        #endregion

        #region Обновление данных за текущий бой
        
        /// <summary>
        /// Участник совершил бросок
        /// </summary>
        public void MarkThrow() => Throws.Value += 1;

        /// <summary>
        /// Отметить дайс использованным
        /// </summary>
        /// <remarks>
        /// При повторном использовании перемещается в конец
        /// </remarks>
        /// <param name="diceId">Идентификатор дайса из сборки</param>
        /// <param name="targetParticipantId">Идентификатор целевого участника</param>
        public void MarkDiceUsed(string diceId, string targetParticipantId)
        {
            bool firstTime = !GetUsedDices().Contains(diceId);
            GetOrCreateDiceUsageState(diceId).MarkUsed(Throws.Value, targetParticipantId);
            onDiceUsed?.Invoke(diceId, firstTime);
        }
        
        /// <summary>
        /// Отметить расходник использованным
        /// </summary>
        /// <remarks>
        /// При повторном использовании перемещается в конец
        /// </remarks>
        /// <param name="consumableId">Идентификатор расходника из сборки</param>
        /// <param name="targetParticipantId">Идентификатор целевого участника</param>
        public void MarkConsumableUsed(string consumableId, string targetParticipantId)
        {
            bool firstTime = !GetUsedConsumables().Contains(consumableId);
            GetOrCreateConsumableUsageState(consumableId).MarkUsed(Throws.Value, targetParticipantId);
            onConsumableUsed?.Invoke(consumableId, firstTime);
        }

        /// <summary>
        /// Учесть результат ролла дайса из сборки
        /// </summary>
        /// <param name="diceId">Идентификатор дайса из сборки</param>
        /// <param name="rawResult">Сырой результат броска</param>
        public void RegisterRawRollResult(string diceId, int rawResult)
        {
            RollHistoryEntry entry = new(Throws.Value, diceId, rawResult);
            rollHistory[diceId] = entry;
        }
        
        #endregion
    }
}