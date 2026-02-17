using System.Collections.Generic;
using Blackset.Data.Base;
using Blackset.Data.Configurations;
using Blackset.Data.Items.Types;
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
        /// Визуальный стиль дайса
        /// </summary>
        public DiceStyle Style => style;
        /// <summary>
        /// Конфигурация чисел на гранях
        /// </summary>
        public SideNumbersConfig NumbersConfig => numbersConfig;

        [Header("Стиль")]
        [SerializeField]
        private DiceStyle style = default;
        [Header("Грани")]
        [SerializeField]
        private SideNumbersConfig numbersConfig = default;
    }
}