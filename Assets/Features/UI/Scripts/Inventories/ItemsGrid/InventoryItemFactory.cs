using System.Collections.Generic;
using Blackset.Data.Items.Types;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using UnityEngine;
using Extensions.Log;
using Unity.VisualScripting;

namespace Blackset.UI.InventoryManagement
{
    /// <summary>
    /// Базовый класс UI фабрики содержимого инвентаря
    /// </summary>
    /// <typeparam name="TInventory">Инвентарь</typeparam>
    /// <typeparam name="TItemCell">Ячейка инвентаря</typeparam>
    /// <typeparam name="TData">Данные предмета инвентаря</typeparam>
    /// <typeparam name="TItemType">Тип предметов инвентаря</typeparam>
    public class InventoryItemFactory : MonoBehaviour
    { // TODO Реалиовать обновление определенной ячейки без полного перепостроения + заселение элементами из пула (для этого Dictionary-индекс Id/индекс ячейки)
        /// <summary>
        /// Инвентарь, данные которого выводятся
        /// </summary>
        public Inventory Inventory => inventory;
        
        [Header("Данные"), Space]
        [SerializeField]
        protected Inventories.Inventory inventory;

        [Header("Фильтрация"), Space]
        [SerializeField] 
        [Tooltip("Тип предмета, выводимый фабрикой (оставить пустым для вывода всего содержимого)")]
        protected InventoryItemType filterItemType;

        [Header("UI элемент"), Space]
        [SerializeField]
        [Tooltip("Префаб выводимого элемента")]
        protected InventoryItemElement itemElementPrefab;
        [SerializeField]
        [Tooltip("Корень заспавненных элементов")]
        protected Transform elementsRoot;

        [Header("Опции заселения"), Space]
        [SerializeField]
        protected bool rebuildOnStart = true;
        [SerializeField]
        protected bool rebuildOnEnable = true;
        [SerializeField]
        protected bool rebuildOnInventoryDataUpdated = true;

        /// <summary>
        /// Заселять ли при включении объекта
        /// </summary>
        public bool RebuildOnEnable => rebuildOnEnable;

        protected void Awake()
        {
            Clear();
            PrepareDropZone();
        }
        
        protected void Start()
        {
            if (rebuildOnStart)
            {
                Rebuild();
            }
        }
        
        protected virtual void OnEnable()
        {
            if (inventory != null && rebuildOnInventoryDataUpdated) inventory.onDataUpdated += Rebuild;
            if (rebuildOnEnable) Rebuild();
        }
        
        protected virtual void OnDisable()
        {
            if (inventory != null && rebuildOnInventoryDataUpdated) inventory.onDataUpdated -= Rebuild;
        }

        /// <summary>
        /// Популяция фабрикой
        /// </summary>
        public void Rebuild()
        {
            if (inventory == null || itemElementPrefab == null)
            {
                ServiceDebug.LogError($"{name}: не все ссылки заполнены");
                return;
            }

            Clear();
            
            Populate(inventory.Data);
        }

        protected void Populate(List<InventoryCell> data)
        {
            if (data == null)
            {
                ServiceDebug.LogError($"{name}: данные отсутствуют");
                return;
            }

            foreach (InventoryCell item in data)
            {
                if (item == null)
                {
                    continue;
                }
                if (filterItemType != null && item.ItemTypeId != filterItemType.Id)
                {
                    continue;
                }

                var instance = Instantiate(itemElementPrefab, elementsRoot);

                if (instance != null)
                {
                    instance.InitializeElement(inventory, item.Id);
                }
            }
        }

        protected void Clear()
        {
            if (elementsRoot == null)
            {
                ServiceDebug.LogError($"{name}: ссылка на корень элементов UI не заполнена");
                return;
            }

            for (int i = elementsRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(elementsRoot.GetChild(i).gameObject);
            }
        }

        protected void PrepareDropZone()
        {
            InventoryItemElement dropZone = transform.AddComponent<InventoryItemElement>();
            dropZone.InitializeElement(inventory);
        }
    }
}
