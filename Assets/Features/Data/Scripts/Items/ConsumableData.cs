using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.Data.Items.Visual;
using UnityEngine;

namespace Blackset.Data
{
    /// <summary>
    /// Игровые данные расходника
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Items/Consumable",
        fileName = nameof(ConsumableData))]
    public sealed class ConsumableData : BaseData 
    {
        /// <summary>
        /// Тип расходника
        /// </summary>
        public ConsumableType Type => type;

        /// <summary>
        /// Набор, к которому относится расходник
        /// </summary>
        public ConsumableSet Set => set;

        /// <summary>
        /// Визуальный префаб расходника
        /// </summary>
        public VisualConsumable VisualConsumable => visualConsumable;

        [Header("Тип")]
        [SerializeField]
        private ConsumableType type = default;

        [Header("Набор")]
        [SerializeField]
        private ConsumableSet set = default;

        [Header("Визуал")]
        [SerializeField]
        private VisualConsumable visualConsumable = default;
    }
}