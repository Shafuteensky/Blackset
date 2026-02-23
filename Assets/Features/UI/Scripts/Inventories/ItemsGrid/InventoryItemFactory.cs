using Blackset.Data.Items.Types;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Extensions.Data.InMemoryData;
using UnityEngine;
using Unity.VisualScripting;

namespace Blackset.UI.InventoryManagement
{
    /// <summary>
    /// Базовый класс UI фабрики содержимого инвентаря
    /// </summary>
    public class InventoryItemFactory : BaseInMemoryDataFactory<InventoryItemElement, InventoryCell, Inventory>
    { // TODO Реалиовать обновление определенной ячейки без полного перепостроения + заселение элементами из пула (для этого Dictionary-индекс Id/индекс ячейки)
        
        [Header("Фильтрация"), Space]
        [SerializeField] 
        [Tooltip("Тип предмета, выводимый фабрикой (оставить пустым для вывода всего содержимого)")]
        protected InventoryItemType filterItemType;

        protected void Awake() => PrepareDropZone();
        
        protected void PrepareDropZone()
        {
            InventoryItemElement dropZone = transform.AddComponent<InventoryItemElement>();
            dropZone.InitializeElement(dataContainer);
        }
        
        protected override bool OnValidateItem(InventoryCell item)
        {
            bool isItemFiltered = filterItemType != null && item.ItemTypeId != filterItemType.Id;
            return isItemFiltered;
        }

        protected override void OnInstanceInitialization(InventoryItemElement instance, InventoryCell item)
        {
            instance.InitializeElement(dataContainer, item.Id);
        }
    }
}
