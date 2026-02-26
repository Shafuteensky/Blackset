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
        /// <summary>
        /// Сдался в бою
        /// </summary>
        public bool HasGivenUp;
        /// <summary>
        /// Может действовать
        /// </summary>
        public bool CanAct;
        
        /// <summary>
        /// Количество совершенных бросков
        /// </summary>
        public int Throws;
        /// <summary>
        /// Счет боя 
        /// </summary>
        public int Score;

        /// <summary>
        /// Состояния использования дайсов <id_дайса_в_сборке, использован_ли>
        /// </summary>
        public Dictionary<string, bool> DicesStates;
        /// <summary>
        /// Состояния использования расходников <id_расходника_в_сборке, использован_ли>
        /// </summary>
        public Dictionary<string, bool> ConsumablesStates;

        /// <summary>
        /// Порядок использования дайсов (по идентификатору в сборке)
        /// </summary>
        public List<string> DicesUsageOrder;
        /// <summary>
        /// Порядок использования расходников (по идентификатору в сборке)
        /// </summary>
        public List<string> ConsumablesUsageOrder;
        
        /// <summary>
        /// Последний объявленный дайс (идентификатор в сборке)
        /// </summary>
        public string LastDeclaredDice;
        /// <summary>
        /// Последний использованный дайс (идентификатор в сборке; выбранный по факту)
        /// </summary>
        public string LastUsedDice => DicesUsageOrder.Last();
        /// <summary>
        /// Результаты бросков дайсов (без эффектов и прочего — "сырые")
        /// </summary>
        public Dictionary<string, int> RawRollResults;
        /// <summary>
        /// Результат последнего броска
        /// </summary>
        public int LastDiceRollResult => RawRollResults[LastUsedDice];
        
        /// <summary>
        /// Использован ли расходник в этот ход
        /// </summary>
        public bool consumableUsedThisTurn;
        /// <summary>
        /// Последний использованный расходник (идентификатор в сборке)
        /// </summary>
        public string LastUsedConsumable => ConsumablesUsageOrder.Last();
        
        /// <summary>
        /// Сброс данных до изначальных для нового хода (броска дайса)
        /// </summary>
        /// <param name="maxThrows"></param>
        public void ResetForNewFight()
        {
            HasGivenUp = false;
            CanAct = true;

            Throws = 0;
            Score = 0;

            foreach (var key in new List<string>(DicesStates.Keys))
            {
                DicesStates[key] = false; 
            }
            foreach (var key in new List<string>(ConsumablesStates.Keys))
            {
                ConsumablesStates[key] = false; 
            }

            DicesUsageOrder = new();
            ConsumablesUsageOrder = new();
            
            LastDeclaredDice = String.Empty;
            RawRollResults.Clear();
            
            consumableUsedThisTurn = false;
        }

        /// <summary>
        /// Отметить участника как спасовавшего
        /// </summary>
        public void MarkGivenUp()
        {
            HasGivenUp = true;
        }

        /// <summary>
        /// Отметить использование расходника на этом ходу
        /// </summary>
        public void UseConsumable()
        {
            consumableUsedThisTurn = true;
        }
    }
}