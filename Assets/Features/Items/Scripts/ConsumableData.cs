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
        fileName = nameof(ConsumableData))]
    public sealed class ConsumableData : EffectingItem 
    {
        /// <summary>
        /// Визуальный префаб расходника
        /// </summary>
        public VisualConsumable VisualConsumable => visualConsumable;

        [Header("Визуал"), Space]
        [SerializeField] private VisualConsumable visualConsumable;
    }
}