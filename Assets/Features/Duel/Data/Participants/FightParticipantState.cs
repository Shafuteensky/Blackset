using System;
using System.Collections.Generic;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Состояние участника дуэли на текущую битву
    /// </summary>
    public class FightParticipantState
    {
        #region События

        /// <summary>
        /// Участник сдался в текущем бою
        /// </summary>
        public event Action onGiveUp; 
        /// <summary>
        /// Игровой счет текущего боя обновился
        /// </summary>
        public event Action onScoreUpdate; 
        /// <summary>
        /// Дайс из сборки использован в текущем бою
        /// </summary>
        public event Action onDiceUsed; 
        /// <summary>
        /// Расходник из сборки использован в текущем бою
        /// </summary>
        public event Action onConsumableUsed; 
        
        #endregion
        
        /// <summary>
        /// Сдался в текущем бою
        /// </summary>
        public bool HasGivenUp { get; private set; }
        
        /// <summary>
        /// Количество совершенных бросков
        /// </summary>
        public int Throws { get; private set; }
        /// <summary>
        /// Счет боя 
        /// </summary>
        public int Score { get; private set; }

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

        private readonly List<string> dicesUsed = new();
        private readonly List<string> consumablesUsed = new();
        
        private readonly Dictionary<string, int> rawRollResults = new();
        private TurnParticipantState turnState = new();

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
            HasGivenUp = false;

            Throws = 0;
            Score = 0;

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
            Throws += 1;
        }

        /// <summary>
        /// Обновить счет участника
        /// </summary>
        /// <param name="newScore">Новое значение счета</param>
        public void UpdateScore(int newScore)
        {
            Score = newScore;
            onScoreUpdate?.Invoke();
        }
        
        /// <summary>
        /// Отметить участника как сдавшегося в этом бою
        /// </summary>
        public void MarkGivenUp()
        {
            HasGivenUp = true;
            onGiveUp?.Invoke();
        }

        /// <summary>
        /// Отметить дайс использованным
        /// </summary>
        /// <param name="dice">Идентификатор дайса из сборки</param>
        public void MarkDiceUsed(string dice)
        {
            dicesUsed.Add(dice);
            onDiceUsed?.Invoke();
        }

        /// <summary>
        /// Отметить расходник использованным
        /// </summary>
        /// <param name="dice">Идентификатор расходника из сборки</param>
        public void MarkConsumableUsed(string consumable)
        {
            consumablesUsed.Add(consumable);
            onConsumableUsed?.Invoke();
        }

        /// <summary>
        /// Учесть результат ролла дайса из сборки
        /// </summary>
        /// <param name="dice">Идентификатор дайса из сборки</param>
        /// <param name="rawResult">Сырой результат броска</param>
        public void RegisterRawRollResult(string dice, int rawResult)
        {
            rawRollResults[dice] = rawResult;
        }
        
        #endregion
    }
}