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
        fileName = nameof(DiceItem))]
    public sealed class DiceItem : EffectingItem
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
        [Tooltip("Стоимость бюджета сборки")]
        private int budgetPrice = 1;
        
        [Header("Стиль")]
        [SerializeField]
        private DiceStyle style;
        [Header("Грани")]
        [SerializeField]
        private SideNumbersConfig numbersConfig;
    }
}