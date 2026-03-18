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
    public sealed class DiceData : EffectingItem
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

        [Tooltip("Стоимость бюджета сборки"), Space]
        [Range(0, 10)]
        [SerializeField] private int budgetPrice = 1;
        
        [Header("Грани"), Space]
        [SerializeField] private SideNumbersConfig numbersConfig;
        
        [Header("Визуальный вид"), Space]
        [SerializeField] private DiceStyle style;
    }
}