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
        /// Все ли обязательные действия за ход выполнены
        /// </summary>
        public bool AllActionsDone => IsDiceChosen.Value;

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
        public ReactiveProperty<string> SelectedDice { get; private set; } = new(string.Empty);

        /// <summary>
        /// Использован ли расходник в этот ход
        /// </summary>
        public ReactiveProperty<bool> IsConsumableChosen { get; private set; } = new(false);

        /// <summary>
        /// Выбранный для использования в этом ходу расходник
        /// </summary>
        public ReactiveProperty<string> SelectedConsumable { get; private set; } = new(string.Empty);

        /// <summary>
        /// Выбранный целевой участник для применения расходника в этом ходу
        /// </summary>
        public ReactiveProperty<string> SelectedTargetParticipantId { get; private set; } = new(string.Empty);

        /// <summary>
        /// Модификаторы текущего броска
        /// </summary>
        public CurrentRollModifiersState RollModifiers { get; private set; } = new();

        private FightParticipantState fightState;

        public void InitFightState(FightParticipantState fightState) => this.fightState = fightState;

        #region Применение данных

        /// <summary>
        /// Сброс данных до изначальных для нового хода
        /// </summary>
        public void ResetForNewTurn()
        {
            HasPassed.Value = false;

            ClearDeclaredDice();
            ClearSelectedDice();
            ClearSelectedConsumable();

            RollModifiers.Reset();
        }

        /// <summary>
        /// Создание копии инстанса
        /// </summary>
        public TurnParticipantState Clone()
        {
            TurnParticipantState clone = new();

            clone.HasPassed.Value = HasPassed.Value;
            clone.IsDiceDeclared.Value = IsDiceDeclared.Value;
            clone.DeclaredDice.Value = DeclaredDice.Value;
            clone.IsDiceChosen.Value = IsDiceChosen.Value;
            clone.SelectedDice.Value = SelectedDice.Value;
            clone.IsConsumableChosen.Value = IsConsumableChosen.Value;
            clone.SelectedConsumable.Value = SelectedConsumable.Value;
            clone.SelectedTargetParticipantId.Value = SelectedTargetParticipantId.Value;

            clone.RollModifiers = RollModifiers.Clone();

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
        public void DeclareDice(SelectionState selection)
        {
            if (!selection.IsItemSelected)
            {
                ClearDeclaredDice();
                return;
            }

            DeclaredDice.Value = selection.SelectedItemId;
            IsDiceDeclared.Value = true;
        }

        /// <summary>
        /// Снять объявление дайса
        /// </summary>
        public void ClearDeclaredDice()
        {
            DeclaredDice.Value = String.Empty;
            IsDiceDeclared.Value = false;
        }

        /// <summary>
        /// Переключить объявленный дайс
        /// </summary>
        public void ToggleDeclaredDice(string diceId)
        {
            if (DeclaredDice.Value == diceId)
            {
                ClearDeclaredDice();
                return;
            }

            DeclareDice(new SelectionState(diceId));
        }

        /// <summary>
        /// Отметка выбранного дайса
        /// </summary>
        public void SelectDice(SelectionState selection)
        {
            if (!selection.IsItemSelected)
            {
                ClearSelectedDice();
                return;
            }

            SelectedDice.Value = selection.SelectedItemId;
            IsDiceChosen.Value = true;
        }

        /// <summary>
        /// Снять выбор дайса
        /// </summary>
        public void ClearSelectedDice()
        {
            SelectedDice.Value = String.Empty;
            IsDiceChosen.Value = false;
        }

        /// <summary>
        /// Переключить выбранный дайс
        /// </summary>
        public void ToggleSelectedDice(string diceId)
        {
            if (SelectedDice.Value == diceId)
            {
                ClearSelectedDice();
                return;
            }

            SelectDice(new SelectionState(diceId));
        }

        /// <summary>
        /// Отметка выбранного расходника
        /// </summary>
        public void SelectConsumable(SelectionState selection)
        {
            if (fightState != null && fightState.IsConsumableUsed(selection.SelectedItemId)) return;

            if (!selection.IsItemSelected)
            {
                ClearSelectedConsumable();
                return;
            }

            SelectedConsumable.Value = selection.SelectedItemId;
            IsConsumableChosen.Value = true;
        }

        /// <summary>
        /// Снять выбор расходника
        /// </summary>
        public void ClearSelectedConsumable()
        {
            SelectedConsumable.Value = String.Empty;
            IsConsumableChosen.Value = false;
        }

        /// <summary>
        /// Переключить выбранный расходник
        /// </summary>
        public void ToggleSelectedConsumable(string consumableId)
        {
            if (SelectedConsumable.Value == consumableId)
            {
                ClearSelectedConsumable();
                return;
            }

            SelectConsumable(new SelectionState(consumableId));
        }

        /// <summary>
        /// Установить выбранную цель применения расходника
        /// </summary>
        public void SetSelectedTargetParticipantId(string targetParticipantId)
        {
            SelectedTargetParticipantId.Value = string.IsNullOrEmpty(targetParticipantId)
                ? String.Empty
                : targetParticipantId;
        }

        #endregion

        #region Помошь

        /// <summary>
        /// Убедиться, что цель применения установлена
        /// </summary>
        public void EnsureSelectedTargetParticipantId(string defaultParticipantId)
        {
            if (string.IsNullOrEmpty(SelectedTargetParticipantId.Value))
                SelectedTargetParticipantId.Value = defaultParticipantId;
        }

        #endregion
    }
}