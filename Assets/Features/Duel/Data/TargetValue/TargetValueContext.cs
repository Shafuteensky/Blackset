using System;
using System.Collections.Generic;
using Blackset.Data.Items.Types;
using Extensions.Log;

namespace Blackset.Duel.TargetValue
{
    /// <summary>
    /// Данные о целевом значении
    /// </summary>
    public class TargetValueContext
    {
        #region События
        
        /// <summary>
        /// Установка величины целевого значения
        /// </summary>
        public event Action onTargetValueSet; 
        
        #endregion

        /// <summary>
        /// Целевое значение
        /// </summary>
        public int TargetValue  
        {
            get
            {
                if (targetValue == 0) ServiceDebug.LogWarning($"Целевое значение равно нулю, возможно не был вызван {nameof(SetTargetValue)}");
                return targetValue;
            }
        }
        /// <summary>
        /// Броски-источники результатов, составивших ЦЗ
        /// </summary>
        public Dictionary<DiceType, int> SourceRolls { get; private set; } = new();

        private int targetValue;
        private bool isFixedValue;

        /// <summary>
        /// Установка величины целевого значения
        /// </summary>
        /// <param name="newTargetValue"></param>
        public void SetTargetValue(int newTargetValue, bool isFixed, Dictionary<DiceType, int> rolls = null)
        {
            targetValue = newTargetValue;
            isFixedValue = isFixed;
            onTargetValueSet?.Invoke();

            if (!isFixed && rolls != null)
            {
                SourceRolls = rolls;
            }
        }
    }
}