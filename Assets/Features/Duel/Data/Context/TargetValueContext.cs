using System;
using System.Collections.Generic;
using Blackset.Data.Items.Types;
using Extensions.Log;

namespace Blackset.Duel.Context
{
    /// <summary>
    /// Данные о целевом значении дуэли
    /// </summary>
    public struct TargetValueContext
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
                if (targetValue == 0) ServiceDebug.LogError($"Целевое значение не установлено, требуется вызов {nameof(SetTargetValue)}");
                return targetValue;
            }
        }
        // TODO public TargetValueMode targetValueMode (смотреть Storm class)
        /// <summary>
        /// Броски-источники результатов, составивших ЦЗ
        /// </summary>
        public Dictionary<DiceType, int> SourceRolls { get; private set; }

        private int targetValue;
        
        /// <summary>
        /// Установка величины целевого значения
        /// </summary>
        /// <param name="newTargetValue"></param>
        public void SetTargetValue(int newTargetValue, Dictionary<DiceType, int> rolls = null)
        {
            targetValue = newTargetValue;
            onTargetValueSet?.Invoke();

            if (rolls != null)
            {
                SourceRolls = rolls;
                // targetValueMode = TargetValueMode.????????? // TODO или передавать его параметром сразу? ДА
            }
        }
    }
}