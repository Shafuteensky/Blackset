using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Результаты бросков дайсов (без эффектов и прочего — "сырые") [id_дайса_в_сборке, результат]
        /// </summary>
        public Dictionary<string, int> RawRollResults => rawRollResults;

        /// <summary>
        /// Состояние на текущий ход
        /// </summary>
        public TurnParticipantState TurnState => turnState;
        
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

        private readonly List<string> dicesUsed = new();
        private readonly List<string> consumablesUsed = new();

        private readonly Dictionary<string, int> rawRollResults = new();
        private readonly TurnParticipantState turnState = new();

        /// <summary>
        /// Создание хранилища данных о состоянии участника дуэли во время битвы
        /// </summary>
        public FightParticipantState() => ResetForNewFight();

        #region Получение данных
        
        /// <summary>
        /// Получить список идентификаторов использованных за бой дайсов
        /// </summary>
        public List<string> GetUsedDices()
        {
            List<string> usedDices = new();
            foreach (var item in dicesUsed)
                usedDices.Add(item);
            return usedDices;
        }

        /// <summary>
        /// Получить список идентификаторов использованных за бой расходников
        /// </summary>
        public List<string> GetUsedConsumables()
        {
            List<string> usedConsumables = new();
            foreach (var item in consumablesUsed)
                usedConsumables.Add(item);
            return usedConsumables;
        }

        /// <summary>
        /// Получить идентификатор последнего использованного за бой дайса
        /// </summary>
        public string GetLastUsedDice() => dicesUsed.Last();

        /// <summary>
        /// Получить идентификатор последнего использованного за бой расходника
        /// </summary>
        public string GetLastUsedConsumable() => consumablesUsed.Last();
        
        /// <summary>
        /// Использован ли дайс за этот бой
        /// </summary>
        /// <param name="dice">Идентификатор проверяемого дайса</param>
        /// <returns>true если был использован хоть раз, иначе false</returns>
        public bool IsDiceUsed(string dice) => dicesUsed.Contains(dice);
        
        /// <summary>
        /// Использован ли расходник за этот бой
        /// </summary>
        /// <param name="consumable">Идентификатор проверяемого расходника</param>
        /// <returns>true если был использован хоть раз, иначе false</returns>
        public bool IsConsumableUsed(string consumable) => consumablesUsed.Contains(consumable);
        
        #endregion
        
        #region Сброс данных
        
        /// <summary>
        /// Сброс данных до изначальных для нового боя
        /// </summary>
        /// <param name="maxThrows"></param>
        public void ResetForNewFight()
        {
            HasGivenUp.Value = false;

            Throws.Value = 0;
            FightScore.Value = 0;

            dicesUsed.Clear();
            rawRollResults.Clear();
            
            turnState.ResetForNewTurn();
            turnState.InitFightState(this);
        }
        
        #endregion

        #region Обновление данных за текущий бой
        
        /// <summary>
        /// Участник совершил бросок
        /// </summary>
        public void MarkThrow()
        {
            Throws.Value += 1;
        }

        /// <summary>
        /// Отметить дайс использованным
        /// </summary>
        /// <remarks>
        /// При повторном использовании перемещается в конец
        /// </remarks>
        /// <param name="diceId">Идентификатор дайса из сборки</param>
        public void MarkDiceUsed(string diceId)
        {
            bool firstTime = !dicesUsed.Contains(diceId);
    
            dicesUsed.Add(diceId);
            onDiceUsed?.Invoke(diceId, firstTime);
        }
        
        /// <summary>
        /// Отметить расходник использованным
        /// </summary>
        /// <remarks>
        /// При повторном использовании перемещается в конец
        /// </remarks>
        /// <param name="consumableId">Идентификатор расходника из сборки</param>
        public void MarkConsumableUsed(string consumableId)
        {
            bool firstTime = !consumablesUsed.Contains(consumableId);
    
            consumablesUsed.Add(consumableId);
            onConsumableUsed?.Invoke(consumableId, firstTime);
        }

        /// <summary>
        /// Учесть результат ролла дайса из сборки
        /// </summary>
        /// <param name="dice">Идентификатор дайса из сборки</param>
        /// <param name="rawResult">Сырой результат броска</param>
        public void RegisterRawRollResult(string dice, int rawResult) => rawRollResults[dice] = rawResult;
        
        #endregion

        /// <summary>
        /// Обновление счета
        /// </summary>
        /// <param name="snapshotScore">Новое значение счета</param>
        public void UpdateScore(int snapshotScore) => FightScore.Value = snapshotScore;
    }
}