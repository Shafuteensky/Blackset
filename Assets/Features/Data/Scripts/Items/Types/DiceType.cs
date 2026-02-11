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
    public sealed class DiceType : BaseItemType
    {
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
        [Range(2, 100)]
        [Tooltip("Количество граней")]
        private int sidesNumber;
        
        [SerializeField]
        [Tooltip("Визуальное представление (префаб базовой модели)")]
        private VisualDice visualDice = default;
    }
}