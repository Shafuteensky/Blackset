using System;
using System.Collections.Generic;
using System.Linq;
using Blackset.Data;
using Blackset.Data.Items.Types;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Состояние участника дуэли на текущую битву
    /// </summary>
    public struct FightParticipantState
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
        /// Использованные за бой дайсы в порядке применения <id_дайса_в_сборке>
        /// </summary>
        public List<string> DicesUsed { get; }
        /// <summary>
        /// Использованные за бой расходники в порядке применения <id_расходника_в_сборке>
        /// </summary>
        public List<string> ConsumablesUsed { get; }
        
        /// <summary>
        /// Результаты бросков дайсов (без эффектов и прочего — "сырые")
        /// </summary>
        public Dictionary<string, int> RawRollResults { get; }
        
        /// <summary>
        /// Состояние на текущий ход
        /// </summary>
        public TurnParticipantState TurnState { get; }

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

            DicesUsed.Clear();
            ConsumablesUsed.Clear();
            
            RawRollResults.Clear();
            
            TurnState.ResetForNewTurn();
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
        /// <param name="delta">Добавочная величина</param>
        public void UpdateScore(int delta)
        {
            Score += delta;
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
            DicesUsed.Add(dice);
            onDiceUsed?.Invoke();
        }

        /// <summary>
        /// Отметить расходник использованным
        /// </summary>
        /// <param name="dice">Идентификатор расходника из сборки</param>
        public void MarkConsumableUsed(string consumable)
        {
            ConsumablesUsed.Add(consumable);
            onConsumableUsed?.Invoke();
        }

        /// <summary>
        /// Учесть результат ролла дайса из сборки
        /// </summary>
        /// <param name="dice">Идентификатор дайса из сборки</param>
        /// <param name="rawResult">Сырой результат броска</param>
        public void RegisterRawRollResult(string dice, int rawResult)
        {
            RawRollResults.Add(dice, rawResult);
        }
        
        #endregion

        #region Internal

        private void SetItems()
        {
            
        }
        
        #endregion
    }
}