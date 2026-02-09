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
        /// Набор, к которому относится дайс
        /// </summary>
        public DiceSet Set => set;

        /// <summary>
        /// Визуальный стиль дайса
        /// </summary>
        public DiceStyle Style => style;

        /// <summary>
        /// Доступные типы дайсов для этих данных
        /// </summary>
        public List<DiceType> AvailableTypes => availableTypes;

        /// <summary>
        /// Конфигурация чисел на гранях
        /// </summary>
        public SideNumbersConfig NumbersConfig => numbersConfig;

        [Header("Типы")]
        [SerializeField]
        private List<DiceType> availableTypes = new List<DiceType>();
        
        [Header("Набор")]
        [SerializeField]
        private DiceSet set = default;

        [Header("Стиль")]
        [SerializeField]
        private DiceStyle style = default;

        [Header("Грани")]
        [SerializeField]
        private SideNumbersConfig numbersConfig = default;
    }
}