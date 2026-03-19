using Blackset.Data.Registries;
using Blackset.Inventories;
using UnityEngine;

namespace Blackset.Shop
{
    /// <summary>
    /// Очистка презентационного инвентаря на <see cref="OnDisable"/>
    /// </summary>
    public sealed class PresenterInventoryCleaner : MonoBehaviour
    {
        private Inventory presenterInventory;

        private void Awake()
        {
            GameData gameData = GameData.Instance;
            presenterInventory = gameData.NewItemsPresenterInventory;
        }
        
        private void OnDisable()
        {
            presenterInventory.Clear();
        }
    }
}