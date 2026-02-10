using System.Collections.Generic;
using Blackset.Inventory.Inventories;
using Blackset.Inventory.ItemPools;
using Extensions.Data.InMemoryData;
using Extensions.Log;
using UnityEngine;

namespace Blackset.Player
{
    /// <summary>
    /// Контейнер мета-данных игрока
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(PlayerMetaDataContainer),
        menuName = "Blackset/Player/" + nameof(PlayerMetaDataContainer))]
    public class PlayerMetaDataContainer : InMemorySingleDataContainer<PlayerMetaData>
    {
        [Header("Дайсы"), Space]
        [SerializeField]
        protected DicesInventory dicesInventory;
        [SerializeField]
        protected List<DicesPoolRow> dicesPool;
        
        [Header("Расходники"), Space]
        [SerializeField]
        protected ConsumablesInventory consumablesInventory;
        [SerializeField]
        protected ConsumablesPool consumablesPool;
        
        protected override void OnInitialize()
        {
            if (dicesInventory == null || dicesPool == null ||
                consumablesInventory == null || consumablesPool == null)
            {
                ServiceDebug.LogError("Ошибка инициализации данных игрока: не все поля заполнены");
                return;
            }
        }
    }
}