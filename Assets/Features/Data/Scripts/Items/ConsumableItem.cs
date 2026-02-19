using Blackset.Data.Items.Visual;
using Blackset.Effects;
using UnityEngine;

namespace Blackset.Data
{
    /// <summary>
    /// Игровые данные расходника
    /// </summary>
    [CreateAssetMenu(
        menuName = "Blackset/Items/Consumable",
        fileName = nameof(ConsumableItem))]
    public sealed class ConsumableItem : EffectingItem 
    {
        /// <summary>
        /// Визуальный префаб расходника
        /// </summary>
        public VisualConsumable VisualConsumable => visualConsumable;

        [Header("Визуал")]
        [SerializeField]
        private VisualConsumable visualConsumable;
    }
}