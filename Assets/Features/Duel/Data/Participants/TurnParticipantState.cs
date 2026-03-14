using System;
using Blackset.Duel.Targets;
using Extensions.Log;
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
            && !String.IsNullOrEmpty(ChosenDice.Value)
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
        public ReactiveProperty<string> ChosenDice { get; private set; } = new(string.Empty);
        
        /// <summary>
        /// Использован ли расходник в этот ход
        /// </summary>
        public ReactiveProperty<bool> IsConsumableChosen { get; private set; } = new(false);
        /// <summary>
        /// Выбранный для использования в этом ходу расходник
        /// </summary>
        public ReactiveProperty<string> ChosenConsumable { get; private set; } = new(string.Empty);
        /// <summary>
        /// Цель применения расходника
        /// </summary>
        public ReactiveProperty<ApplyTarget> ConsumableTarget { get; private set; } = new(ApplyTarget.None);

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
            ChosenDice.Value = String.Empty;
                
            IsConsumableChosen.Value = false;
            ChosenConsumable.Value = String.Empty;
            ConsumableTarget.Value = ApplyTarget.None;
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
        
        /// <summary>
        /// Создание копии инстанса
        /// </summary>
        public TurnParticipantState Clone()
        {
            var clone = new TurnParticipantState();
            clone.ApplyState(this);
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
        public void DeclareDice(string diceId)
        {
            if (string.IsNullOrEmpty(diceId)) return;
            
            DeclaredDice.Value = diceId;
            IsDiceDeclared.Value = true;
        }

        /// <summary>
        /// Отметка выбранного дайса
        /// </summary>
        /// <param name="diceId">Идентификатор дайса</param>
        public void ChoseDice(string diceId)
        {
            if (string.IsNullOrEmpty(diceId)) return;
            
            ChosenDice.Value = diceId;
            IsDiceChosen.Value = true;
        }

        /// <summary>
        /// Отметка выбранного расходника
        /// </summary>
        /// <param name="consumableId">Идентификатор расходника</param>
        public void ChoseConsumable(string consumableId, ApplyTarget target = ApplyTarget.Self)
        {
            ServiceGuard.NotNullOrEmpty(consumableId, nameof(consumableId));
            
            ConsumableTarget.Value = target;
            ChosenConsumable.Value = consumableId;
            IsConsumableChosen.Value = true;
        }

        #endregion
    }
}