using Blackset.Data.Items.Types;
using Blackset.Effects;
using Features.ItemGenerators;
using UnityEngine;

namespace Blackset.Data.Registries
{
    /// <summary>
    /// Фасад всех игровых данных
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(DataRegistriesFacade),
        menuName = "Blackset/Registries/" + nameof(DataRegistriesFacade))]
    public class DataRegistriesFacade : ScriptableObject
    {
        [field: Header("Реестры предметов"), Space]
        /// <summary>
        /// Реестр дайсов
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Реестр дайсов")]
        public InventoryItemsRegistry Dices { get; private set; }
        /// <summary>
        /// Реестр расходников
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Реестр расходников")]
        public InventoryItemsRegistry Consumables { get; private set; }
        
        [field: Header("Реестры типов предметов"), Space]
        /// <summary>
        /// Реестр типов дайсов
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Реестр типов дайсов")]
        public InventoryItemTypesRegistry DiceTypes { get; private set; }
        /// <summary>
        /// Реестр типов расходников
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Реестр типов расходников")]
        public InventoryItemTypesRegistry ConsumableTypes { get; private set; }
    }
}