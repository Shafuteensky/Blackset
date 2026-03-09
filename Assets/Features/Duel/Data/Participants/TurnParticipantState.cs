using System;
using Blackset.Duel.Targets;
using Extensions.Log;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Состояние участника дуэли на текущий ход
    /// </summary>
    public struct TurnParticipantState
    {
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
            && IsConsumableChosen;
        
        /// <summary>
        /// Спасовал
        /// </summary>
        public bool HasPassed { get; private set; }
        
        /// <summary>
        /// Объявлен ли дайс в этот ход
        /// </summary>
        public bool IsDiceDeclared { get; private set; }
        /// <summary>
        /// Объявленный в этом ходу дайс
        /// </summary>
        public string DeclaredDice { get; private set; }
        
        /// <summary>
        /// Использован ли дайс в этот ход
        /// </summary>
        public bool IsDiceChosen { get; private set; }
        /// <summary>
        /// Выбранный для броска в этом ходу дайс
        /// </summary>
        public string ChosenDice { get; private set; }
        
        /// <summary>
        /// Использован ли расходник в этот ход
        /// </summary>
        public bool IsConsumableChosen { get; private set; }
        /// <summary>
        /// Выбранный для использования в этом ходу расходник
        /// </summary>
        public string ChosenConsumable { get; private set; }
        /// <summary>
        /// Цель применения расходника
        /// </summary>
        public ApplyTarget ConsumableTarget { get; private set; }

        #region Применение данных
        
        /// <summary>
        /// Сброс данных до изначальных для нового хода (броска дайса)
        /// </summary>
        /// <param name="maxThrows"></param>
        public void ResetForNewTurn()
        {
            HasPassed = false;

            IsDiceDeclared = false;
            DeclaredDice = String.Empty;
            
            IsDiceChosen = false;
            ChosenDice = String.Empty;
                
            IsConsumableChosen = false;
            ChosenConsumable = String.Empty;
            ConsumableTarget = ApplyTarget.None;
        }
        
        /// <summary>
        /// Применить намерение на действие в ходе
        /// </summary>
        /// <param name="intentState">Состояние намерения</param>
        public void ApplyState(TurnParticipantState intentState)
        {
            HasPassed = intentState.HasPassed;
            
            IsDiceDeclared = intentState.IsDiceDeclared;
            DeclaredDice = intentState.DeclaredDice;
            
            IsDiceChosen = intentState.IsDiceChosen;
            ChosenDice = intentState.ChosenDice;
            
            IsConsumableChosen = intentState.IsConsumableChosen;
            ChosenConsumable = intentState.ChosenConsumable;
            ConsumableTarget = intentState.ConsumableTarget;
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
            IsDiceDeclared = true;
        }

        /// <summary>
        /// Отметка выбранного дайса
        /// </summary>
        /// <param name="diceId">Идентификатор дайса</param>
        public void ChoseDice(string diceId)
        {
            ServiceGuard.NotNullOrEmpty(diceId, nameof(diceId));
            
            ChosenDice = diceId;
            IsDiceChosen = true;
        }

        /// <summary>
        /// Отметка выбранного расходника
        /// </summary>
        /// <param name="consumableId">Идентификатор расходника</param>
        public void ChoseConsumable(string consumableId, ApplyTarget target = ApplyTarget.Self)
        {
            ServiceGuard.NotNullOrEmpty(consumableId, nameof(consumableId));
            
            ConsumableTarget = target;
            ChosenConsumable = consumableId;
            IsConsumableChosen = true;
        }

        #endregion
    }
}