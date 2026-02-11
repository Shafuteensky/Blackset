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
        public DiceDataRegistry Dices { get; private set; }
        /// <summary>
        /// Реестр расходников
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Реестр расходников")]
        public ConsumableDataRegistry Consumables { get; private set; }
        
        [field: Header("Реестры типов предметов"), Space]
        /// <summary>
        /// Реестр типов дайсов
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Реестр типов дайсов")]
        public DiceTypeRegistry DiceTypes { get; private set; }
        /// <summary>
        /// Реестр типов расходников
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Реестр типов расходников")]
        public ConsumableTypeRegistry ConsumableTypes { get; private set; }
    }
}