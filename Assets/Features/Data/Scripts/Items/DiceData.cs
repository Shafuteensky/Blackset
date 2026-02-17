using Blackset.Data.Configurations;
using Blackset.Data.Items.Visual;
using Blackset.Effects;
using UnityEngine;

namespace Blackset.Data
{
    /// <summary>
    /// Игровые данные особого дайса
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Items/Dice",
        fileName = nameof(DiceData))]
    public sealed class DiceData : EffectingItemData
    {
        /// <summary>
        /// Бюджетная стоимость
        /// </summary>
        public int BudgetPrice => budgetPrice;
        
        /// <summary>
        /// Визуальный стиль дайса
        /// </summary>
        public DiceStyle Style => style;
        /// <summary>
        /// Конфигурация чисел на гранях
        /// </summary>
        public SideNumbersConfig NumbersConfig => numbersConfig;

        [SerializeField]
        [Range(1, 10)]
        [Tooltip("Базовая стоимость бюджета сборки (без учета редкости и типа)")]
        private int budgetPrice = 1;
        
        [Header("Стиль")]
        [SerializeField]
        private DiceStyle style;
        [Header("Грани")]
        [SerializeField]
        private SideNumbersConfig numbersConfig;
    }
}