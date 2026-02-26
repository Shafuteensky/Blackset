using System;
using System.Collections.Generic;
using Blackset.Data;
using Blackset.Data.Items.Types;

namespace Blackset.Duel.Participants
{
    /// <summary>
    /// Состояние участника дуэли на текущий ход
    /// </summary>
    public struct TurnParticipantState
    {
        /// <summary>
        /// Счет боя 
        /// </summary>
        public int Score;
        /// <summary>
        /// Спасовал
        /// </summary>
        public bool HasPassed;
        /// <summary>
        /// Может действовать
        /// </summary>
        public bool CanAct;
        /// <summary>
        /// Последний объявленный дайс
        /// </summary>
        public string LastDeclaredDiceType;
        /// <summary>
        /// Последний использованный дайс (выбранный по факту)
        /// </summary>
        public DiceItemContext LastUsedDice;
        /// <summary>
        /// Результат последнего броска
        /// </summary>
        public int LastDiceRollResult;
        /// <summary>
        /// Раскрытые дайсы (использованные хоть раз за дуэль)
        /// </summary>
        public List<DiceType> RevealedDices;
        /// <summary>
        /// Использован ли расходник в этот ход
        /// </summary>
        public bool consumableUsedThisTurn;
        /// <summary>
        /// Последний использованный расходник
        /// </summary>
        public ConsumableItemContext LastUsedConsumable;
        
        /// <summary>
        /// Сброс данных до изначальных для нового хода (броска дайса)
        /// </summary>
        /// <param name="maxThrows"></param>
        public void ResetForNewFight()
        {
            HasPassed = false;
            CanAct = true;
            
            LastDeclaredDiceType = String.Empty;
            LastUsedDice = new();
            LastDiceRollResult = 0;
            
            consumableUsedThisTurn = false;
            LastUsedConsumable = new();
        }

        /// <summary>
        /// Отметить участника как спасовавшего
        /// </summary>
        public void MarkPassed()
        {
            HasPassed = true;
        }

        /// <summary>
        /// Отметить использование расходника на этом ходу
        /// </summary>
        public void UseConsumable()
        {
            consumableUsedThisTurn = true;
        }

        /// <summary>
        /// Раскрыть дайс
        /// </summary>
        public void RevealDice(DiceType diceType)
        {
            RevealedDices.Add(diceType);
        }
    }
}