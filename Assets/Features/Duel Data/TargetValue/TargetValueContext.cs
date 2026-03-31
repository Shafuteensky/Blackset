using System.Collections.Generic;
using Blackset.Data.Items.Types;
using Extensions.Reactive;

namespace Blackset.Duel.TargetValue
{
    /// <summary>
    /// Данные о целевом значении
    /// </summary>
    public class TargetValueContext
    {
        /// <summary>
        /// Целевое значение
        /// </summary>
        public ReactiveProperty<int> TargetValue = new(0);
        /// <summary>
        /// Броски-источники результатов, составивших ЦЗ
        /// </summary>
        public Dictionary<DiceType, int> SourceRolls { get; private set; } = new();
        /// <summary>
        /// Фиксировано ли ЦЗ либо случайно
        /// </summary>
        public bool IsFixedValue { get; private set; }

        /// <summary>
        /// Установка величины целевого значения
        /// </summary>
        /// <param name="newTargetValue"></param>
        public void SetTargetValue(int newTargetValue, bool isFixed, Dictionary<DiceType, int> rolls = null)
        {
            TargetValue.Value = newTargetValue;
            IsFixedValue = isFixed;

            if (!isFixed && rolls != null)
            {
                SourceRolls = rolls;
            }
        }
    }
}