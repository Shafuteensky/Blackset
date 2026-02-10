using System.Collections.Generic;
using Blackset.Inventory.Inventories;
using Blackset.Inventory.ItemPools;
using UnityEngine;

namespace Blackset.Player
{
    /// <summary>
    /// Фасад всех данных игрока
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlayerDataFacade),
        menuName = "Blackset/Player/" + nameof(PlayerDataFacade))]
    public sealed class PlayerDataFacade : ScriptableObject
    {
        [Header("Данные")]
        [field: Space]
        /// <summary>
        /// Контейнер мета-данных
        /// </summary>
        [field:SerializeField]
        [Tooltip("Контейнер мета-данных")]
        public PlayerMetaDataContainer MetaData { get; private set; }
        
        [Header("Инвентари")]
        [field: Space]
        /// <summary>
        /// Инвентарь дайсов
        /// </summary>
        [field:SerializeField]
        [Tooltip("Инвентарь дайсов")]
        public DicesInventory DicesInventory { get; private set; }
        /// <summary>
        /// Инвентарь расходников
        /// </summary>
        [field:SerializeField]
        [Tooltip("Инвентарь расходников")]
        public ConsumablesInventory ConsumablesInventory { get; private set; }
        
        [Header("Пулы")]
        [field: Space]
        /// <summary>
        /// Пул дайсов
        /// </summary>
        [field:SerializeField]
        [Tooltip("Пул дайсов")]
        public List<DicesPoolRow> DicesPoolRows { get; private set; }
        /// <summary>
        /// Пул расходников
        /// </summary>
        [field:SerializeField]
        [Tooltip("Пул расходников")]
        public ConsumablesPool ConsumablesPool { get; private set; }
    }
}
