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
        
        [field: Header("Инвентари"), Space]
        /// <summary>
        /// Инвентарь дайсов
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Инвентарь дайсов")]
        public DicesInventory DicesInventory { get; private set; }
        /// <summary>
        /// Инвентарь расходников
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Инвентарь расходников")]
        public ConsumablesInventory ConsumablesInventory { get; private set; }
        
        [field: Header("Пулы"), Space]
        /// <summary>
        /// Пул дайсов
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Пулы дайсов (по номиналам)")]
        public List<DicesInventory> DicesPoolRows { get; private set; }
        /// <summary>
        /// Пул расходников
        /// </summary>
        [field:SerializeField]
        [field: Tooltip("Пул расходников")]
        public ConsumablesInventory ConsumablesPool { get; private set; }

        /// <summary>
        /// Получить пул дайсов определенного типа
        /// </summary>
        /// <param name="diceType">Тип требуемого пула</param>
        /// <returns>Пул дайсов определенного типа</returns>
        public DicesInventory GetDicePoolRow(DiceType diceType)
        {
            foreach (DicesInventory dicePoolRow in DicesPoolRows)
            {
                if (dicePoolRow.AllowedItemType == diceType) return dicePoolRow;
            }
            
            ServiceDebug.LogError($"Пул дайсов типа {diceType.DataName} не найден");
            return null;
        }
    }
}
