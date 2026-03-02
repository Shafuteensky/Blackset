using System;
using Extensions.Log;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Состояние участника дуэли на текущий ход
    /// </summary>
    public struct TurnParticipantState
    {
        /// <summary>
        /// Спасовал
        /// </summary>
        public bool HasPassed { get; private set; }
        /// <summary>
        /// Может действовать
        /// </summary>
        public bool CanAct => !HasPassed && !AllActionsDone;
        /// <summary>
        /// Все ли возможные действия за ход выполнены
        /// </summary>
        public bool AllActionsDone =>
            !String.IsNullOrEmpty(DeclaredDice)
            && !String.IsNullOrEmpty(ChosenDice)
            && ConsumableChosen;
        
        /// <summary>
        /// Объявленный в этом ходу дайс
        /// </summary>
        public string DeclaredDice { get; private set; }
        /// <summary>
        /// Объявлен ли дайс в этот ход
        /// </summary>
        public bool DiceDeclared { get; private set; }
        /// <summary>
        /// Выбранный для броска в этом ходу дайс
        /// </summary>
        public string ChosenDice { get; private set; }
        /// <summary>
        /// Использован ли дайс в этот ход
        /// </summary>
        public bool DiceChosen { get; private set; }
        
        /// <summary>
        /// Использован ли расходник в этот ход
        /// </summary>
        public bool ConsumableChosen { get; private set; }
        /// <summary>
        /// Выбранный для использования в этом ходу расходник
        /// </summary>
        public string ChosenConsumable { get; private set; }

        #region Сброс данных
        
        /// <summary>
        /// Сброс данных до изначальных для нового хода (броска дайса)
        /// </summary>
        /// <param name="maxThrows"></param>
        public void ResetForNewTurn()
        {
            HasPassed = false;

            DeclaredDice = String.Empty;
            DiceDeclared = false;
            ChosenDice = String.Empty;
            DiceChosen = false;
                
            ChosenConsumable = String.Empty;
            ConsumableChosen = false;
        }
        
        #endregion

        #region Обновление данных за текущий ход

        /// <summary>
        /// Отметить участника как спасовавшего
        /// </summary>
        public void MarkPassed()
        {
            HasPassed = true;
        }

        /// <summary>
        /// Отметка объявленного дайса
        /// </summary>
        /// <param name="diceId">Идентификатор дайса</param>
        public void DeclareDice(string diceId)
        {
            ServiceGuard.NotNullOrEmpty(diceId, nameof(diceId));
            
            DeclaredDice = diceId;
            DiceDeclared = true;
        }

        /// <summary>
        /// Отметка выбранного дайса
        /// </summary>
        /// <param name="diceId">Идентификатор дайса</param>
        public void ChoseDice(string diceId)
        {
            ServiceGuard.NotNullOrEmpty(diceId, nameof(diceId));
            
            ChosenDice = diceId;
            DiceChosen = true;
        }

        /// <summary>
        /// Отметка выбранного расходника
        /// </summary>
        /// <param name="consumableId">Идентификатор расходника</param>
        public void ChoseConsumable(string consumableId)
        {
            ServiceGuard.NotNullOrEmpty(consumableId, nameof(consumableId));
            
            ChosenConsumable = consumableId;
            ConsumableChosen = true;
        }

        #endregion
    }
}