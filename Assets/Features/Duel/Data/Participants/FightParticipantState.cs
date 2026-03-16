using System;
using System.Collections.Generic;
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
        public event Action<string> onConsumableUsed; 
        
        #endregion

        /// <summary>
        /// Использованные за бой дайсы в порядке применения [id_дайса_в_сборке]
        /// </summary>
        public List<string> DicesUsed => dicesUsed;
        /// <summary>
        /// Использованные за бой расходники в порядке применения [id_расходника_в_сборке]
        /// </summary>
        public List<string> ConsumablesUsed => consumablesUsed;

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
        public FightParticipantState()
        {
            ResetForNewFight();
        }
        
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
            consumablesUsed.Clear();
            
            rawRollResults.Clear();
            
            turnState.ResetForNewTurn();
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
        /// <param name="dice">Идентификатор дайса из сборки</param>
        public void MarkDiceUsed(string dice)
        {
            bool firstTime = !dicesUsed.Contains(dice);
            dicesUsed.Add(dice);
            onDiceUsed?.Invoke(dice, firstTime);
        }

        /// <summary>
        /// Отметить расходник использованным
        /// </summary>
        /// <param name="dice">Идентификатор расходника из сборки</param>
        public void MarkConsumableUsed(string consumable)
        {
            consumablesUsed.Add(consumable);
            onConsumableUsed?.Invoke(consumable);
        }

        /// <summary>
        /// Учесть результат ролла дайса из сборки
        /// </summary>
        /// <param name="dice">Идентификатор дайса из сборки</param>
        /// <param name="rawResult">Сырой результат броска</param>
        public void RegisterRawRollResult(string dice, int rawResult)
        {
            if (rawResult == 0) return;
            rawRollResults[dice] = rawResult;
        }
        
        #endregion

        /// <summary>
        /// Обновление счета
        /// </summary>
        /// <param name="snapshotScore">Новое значение счета</param>
        public void UpdateScore(int snapshotScore)
        {
            FightScore.Value = snapshotScore;
        }
    }
}