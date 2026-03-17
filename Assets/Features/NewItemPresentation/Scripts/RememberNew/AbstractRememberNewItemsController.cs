using Blackset.Data.Registries;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using UnityEngine;

namespace Blackset.Shop
{
    /// <summary>
    /// Базовый контроллер учета новых предметов
    /// </summary>
    public abstract class AbstractRememberNewItemsController : MonoBehaviour
    {
        protected Inventory presenterInventory;

        protected virtual void Awake() => presenterInventory = GameData.Instance.NewItemsPresenterInventory;

        protected virtual void RememberNewItem(ItemContext item, int amount) => presenterInventory.AddItem(item, amount);
    }
}