using System;

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
            && consumableUsedThisTurn;
        
        /// <summary>
        /// Объявленный в этом ходу дайс
        /// </summary>
        public string DeclaredDice { get; private set; }
        /// <summary>
        /// Выбранный для броска в этом ходу дайс
        /// </summary>
        public string ChosenDice { get; private set; }
        
        /// <summary>
        /// Использован ли расходник в этот ход
        /// </summary>
        public bool consumableUsedThisTurn => !String.IsNullOrEmpty(ChosenConsumable);
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
            ChosenDice = String.Empty;
                
            ChosenConsumable = String.Empty;
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

        #endregion
    }
}