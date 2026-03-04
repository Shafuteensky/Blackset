using Blackset.Data.Items.Visual;
using UnityEngine;

namespace Blackset.Data.Items.Types
{
    /// <summary>
    /// Тип дайса
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Items/Types/" + nameof(DiceType),
        fileName = nameof(DiceType))]
    public sealed class DiceType : InventoryItemType
    {
        private const int MIN_DICE_SIDE_VALUE = 1;
        
        /// <summary>
        /// Минимальное дефолтное значение грани
        /// </summary>
        public int MinDiceSideValue => MIN_DICE_SIDE_VALUE;
        
        /// <summary>
        /// Является ли тип ограниченным
        /// </summary>
        public bool IsRestricted => isRestricted;
        
        /// <summary>
        /// Количество граней
        /// </summary>
        public int SidesNumber => sidesNumber;
        /// <summary>
        /// Визуальный префаб дайса
        /// </summary>
        public VisualDice VisualDice => visualDice;

        [Header("Параметры типа дайса"), Space]
        
        [SerializeField]
        [Tooltip("Является ли тип ограниченным (только для эффектов, не для генерации наград/лотов)")]
        private bool isRestricted;
        
        [SerializeField]
        [Range(2, 100)]
        [Tooltip("Количество граней")]
        private int sidesNumber;
        
        [SerializeField]
        [Tooltip("Визуальное представление (префаб базовой модели)")]
        private VisualDice visualDice;
    }
}