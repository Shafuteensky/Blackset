using Blackset.Data.Registries;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using UnityEngine;

namespace Blackset.Shop
{
    /// <summary>
    /// Абстракция контроллера вывода информации о новом полученном предмете
    /// </summary>
    public abstract class AbstractNewItemsShowController : MonoBehaviour
    {
        [Header("Параметры показа"), Space]
        [Tooltip("Показать предметы при включении контроллера, если есть новые предметы")]
        [SerializeField] protected bool showOnEnable = true;
        
        protected Inventory presenterInventory;

        protected virtual  void Awake() => presenterInventory = GameData.Instance.NewItemsPresenterInventory;
        
        protected virtual  void OnEnable()
        {
            presenterInventory.onItemAdded += ShowNewItems;

            if (showOnEnable && !presenterInventory.IsEmpty) ShowNewItems();
        }
        
        protected virtual  void OnDisable() => presenterInventory.onItemAdded -= ShowNewItems;

        protected virtual void ShowNewItems(ItemContext _, int __) => ShowNewItems();
        
        protected virtual void ShowNewItems()
        {
            if (presenterInventory.IsEmpty) return;

            OnNewItemsShowRequested();
        }

        protected abstract void OnNewItemsShowRequested();
    }
}