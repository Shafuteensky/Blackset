using System;
using Blackset.DecisionInput;
using Extensions.Reactive;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Состояние участника дуэли на текущий ход
    /// </summary>
    public class TurnParticipantState
    {
        /// <summary>
        /// Может действовать
        /// </summary>
        public bool CanAct => !HasPassed.Value && !AllActionsDone;
        /// <summary>
        /// Все ли возможные действия за ход выполнены
        /// </summary>
        public bool AllActionsDone =>
            !String.IsNullOrEmpty(DeclaredDice.Value)
            && IsConsumableChosen.Value;

        /// <summary>
        /// Спасовал
        /// </summary>
        public ReactiveProperty<bool> HasPassed { get; private set; } = new(false);
        
        /// <summary>
        /// Объявлен ли дайс в этот ход
        /// </summary>
        public ReactiveProperty<bool> IsDiceDeclared { get; private set; } = new(false);
        /// <summary>
        /// Объявленный в этом ходу дайс
        /// </summary>
        public ReactiveProperty<string> DeclaredDice { get; private set; } = new(string.Empty);
        
        /// <summary>
        /// Использован ли дайс в этот ход
        /// </summary>
        public ReactiveProperty<bool> IsDiceChosen { get; private set; } = new(false);
        /// <summary>
        /// Выбранный для броска в этом ходу дайс
        /// </summary>
        public ReactiveProperty<string> SelectedDice { get; private set; } = new();
        
        /// <summary>
        /// Использован ли расходник в этот ход
        /// </summary>
        public ReactiveProperty<bool> IsConsumableChosen { get; private set; } = new(false);
        /// <summary>
        /// Выбранный для использования в этом ходу расходник
        /// </summary>
        public ReactiveProperty<string> SelectedConsumable { get; private set; } = new();

        #region Применение данных
        
        /// <summary>
        /// Сброс данных до изначальных для нового хода (броска дайса)
        /// </summary>
        /// <param name="maxThrows"></param>
        public void ResetForNewTurn()
        {
            HasPassed.Value = false;

            IsDiceDeclared.Value = false;
            DeclaredDice.Value = String.Empty;
            
            IsDiceChosen.Value = false;
            SelectedDice.Value = String.Empty;
                
            IsConsumableChosen.Value = false;
            SelectedConsumable.Value =  String.Empty;
        }
        
        /// <summary>
        /// Создание копии инстанса
        /// </summary>
        public TurnParticipantState Clone()
        {
            var clone = new TurnParticipantState();
            
            clone.HasPassed.Value = HasPassed.Value;
            
            clone.IsDiceDeclared = IsDiceDeclared;
            if (!string.IsNullOrEmpty(DeclaredDice.Value))
                clone.DeclaredDice.Value = DeclaredDice.Value;
            
            clone.IsDiceChosen.Value = IsDiceChosen.Value;
            clone.SelectedDice.Value = SelectedDice.Value;
            
            clone.IsConsumableChosen.Value = IsConsumableChosen.Value;
            clone.SelectedConsumable.Value = SelectedConsumable.Value;
            
            return clone;
        }
        
        #endregion

        #region Обновление данных за текущий ход

        /// <summary>
        /// Отметить участника как спасовавшего
        /// </summary>
        public void MarkPassed()
        {
            HasPassed.Value = true;
        }

        /// <summary>
        /// Отметка объявленного дайса
        /// </summary>
        /// <param name="diceId">Идентификатор дайса</param>
        public void DeclareDice(SelectionState selection)
        {
            if (!selection.IsItemSelected) return;
            
            DeclaredDice.Value = selection.SelectedItemId;
            IsDiceDeclared.Value = selection.IsItemSelected;
        }

        /// <summary>
        /// Отметка выбранного дайса
        /// </summary>
        /// <param name="diceId">Идентификатор дайса</param>
        public void SelectDice(SelectionState selection)
        {
            if (!selection.IsItemSelected) return;
            
            IsDiceChosen.Value = selection.IsItemSelected;
            SelectedDice.Value = selection.SelectedItemId;
        }

        /// <summary>
        /// Отметка выбранного расходника
        /// </summary>
        /// <param name="consumableId">Идентификатор расходника</param>
        public void SelectConsumable(SelectionState selection)
        {
            if (!selection.IsItemSelected) return;
            
            IsConsumableChosen.Value = selection.IsItemSelected;
            SelectedConsumable.Value = selection.SelectedItemId;
        }

        #endregion
    }
}