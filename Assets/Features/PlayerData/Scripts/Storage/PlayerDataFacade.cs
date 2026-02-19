using System.Collections.Generic;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Inventories;
using Extensions.Log;
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
        [field: Header("Данные"), Space]
        /// <summary>
        /// Контейнер мета-данных
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Контейнер мета-данных")]
        public PlayerMetaDataContainer MetaData { get; private set; }
        /// <summary>
        /// Контейнер данных прогресса
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Контейнер данных прогресса")]
        public PlayerProgressDataContainer ProgressData { get; private set; }
        
        [field: Header("Инвентари"), Space]
        /// <summary>
        /// Инвентарь дайсов
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Инвентарь дайсов")]
        public Inventory.Inventories.Inventory DicesInventory { get; private set; }
        /// <summary>
        /// Инвентарь расходников
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Инвентарь расходников")]
        public Inventory.Inventories.Inventory ConsumablesInventory { get; private set; }
        
        [field: Header("Пулы"), Space]
        /// <summary>
        /// Пул дайсов
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Пулы дайсов (по номиналам)")]
        public List<Inventory.Inventories.Inventory> DicesPoolRows { get; private set; }
        /// <summary>
        /// Пул расходников
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Пул расходников")]
        public Inventory.Inventories.Inventory ConsumablesPool { get; private set; }

        /// <summary>
        /// Получить пул дайсов определенного типа
        /// </summary>
        /// <param name="diceType">Тип требуемого пула</param>
        /// <returns>Пул дайсов определенного типа</returns>
        public Inventory.Inventories.Inventory GetDicePoolRow(DiceType diceType)
        {
            foreach (Inventory.Inventories.Inventory dicePoolRow in DicesPoolRows)
            {
                if (dicePoolRow.AllowedItemType == diceType) return dicePoolRow;
            }
            
            ServiceDebug.LogError($"Пул дайсов типа {diceType.DataName} не найден");
            return null;
        }
    }
}
