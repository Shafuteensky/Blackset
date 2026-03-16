using Blackset.Data.Items.Types;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Blackset.Inventories.Scripts.Items;
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
        [SerializeField] protected ItemClass itemClass = ItemClass.Any;
        [Tooltip("Тип предмета, выводимый фабрикой (оставить пустым для вывода всего содержимого)")]
        [SerializeField] protected InventoryItemType filterItemType;

        protected void Awake() => PrepareDropZone();
        
        protected void PrepareDropZone()
        {
            InventoryItemElement dropZone = transform.AddComponent<InventoryItemElement>();
            dropZone.Initialize(dataContainer, null);
        }
        
        protected override bool OnValidateItem(InventoryCell item)
        {
            if (item == null) return true;

            bool classMatches =
                itemClass == ItemClass.Any ||
                item.Item.ItemClass == itemClass;

            bool typeMatches =
                filterItemType == null ||
                item.Item.ItemTypeId == filterItemType.Id;

            return classMatches == false || typeMatches == false;
        }

        protected override void OnInstanceInitialization(InventoryItemElement instance, InventoryCell item, Inventory container)
        {
            instance.Initialize(dataContainer, item.Id);
        }
    }
}
