using Blackset.Data.Base;
using Blackset.Data.Items.Visual;
using UnityEngine;

namespace Blackset.Data.Items.Types
{
    /// <summary>
    /// Тип дайса
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Item types/" + nameof(DiceType),
        fileName = nameof(DiceType))]
    public sealed class DiceType : BaseData
    {
        /// <summary>
        /// Количество граней
        /// </summary>
        public int SidesNumber => sidesNumber;

        /// <summary>
        /// Визуальный префаб дайса
        /// </summary>
        public VisualDice VisualDice => visualDice;

        [Header("Количество граней")]
        [SerializeField]
        private int sidesNumber;

        [Header("Визуальное представление (префаб базовой модели)")]
        [SerializeField]
        private VisualDice visualDice = default;
    }
}