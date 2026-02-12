using System.Collections.Generic;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.Data.Registries;
using Blackset.Inventory.Cells;
using UnityEngine;
using Blackset.Inventory.Inventories;
using Extensions.Log;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Базовый класс UI фабрики содержимого инвентаря
    /// </summary>
    /// <typeparam name="TInventory">Тип инвентаря</typeparam>
    /// <typeparam name="TItemCell">Ячейка инвентаря</typeparam>
    /// <typeparam name="TItemType">Тип предметов инвентаря</typeparam>
    public abstract class GenericInventoryItemFactory<TInventory, TItemCell, TData, TItemType> : MonoBehaviour
        where TInventory : BaseInventory<TItemCell, TData, TItemType>
        where TItemCell : BaseItemCell<TData, TItemType>
        where TData : BaseData
        where TItemType : BaseItemType
    {
        [Header("Данные"), Space]
        [SerializeField]
        protected TInventory inventory;
        [SerializeField]
        protected DataRegistriesFacade gameDataRegistry;

        [Header("Фильтрация"), Space]
        [SerializeField] 
        [Tooltip("Тип предмета, выводимый фабрикой (оставить пустым для вывода всего содержимого)")]
        protected TItemType filterItemType;

        [Header("UI элемент"), Space]
        [SerializeField]
        [Tooltip("Префаб выводимого элемента")]
        protected GenericInventoryItemElement<TItemCell, TData, TItemType> itemElementPrefab;
        [SerializeField]
        [Tooltip("Корень заспавненных элементов")]
        protected Transform elementsRoot;

        [Header("Опции заселения"), Space]
        [SerializeField]
        protected bool rebuildOnStart = true;
        [SerializeField]
        protected bool rebuildOnEnable = true;

        /// <summary>
        /// Заселять ли при включении объекта
        /// </summary>
        public bool RebuildOnEnable => rebuildOnEnable;

        protected void Awake()
        {
            Clear();
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
            if (rebuildOnEnable)
            {
                Rebuild();
            }
        }

        /// <summary>
        /// Популяция фабрикой
        /// </summary>
        public void Rebuild()
        {
            if (inventory == null || gameDataRegistry == null || itemElementPrefab == null)
            {
                ServiceDebug.LogError($"{name}: не все ссылки заполнены");
                return;
            }

            Clear();
            
            IReadOnlyList<TItemCell> data = inventory.Data;
            Rebuild(data);
        }

        protected void Rebuild(IReadOnlyList<TItemCell> data)
        {
            if (data == null)
            {
                ServiceDebug.LogError($"{name}: данные отсутствуют");
                return;
            }

            foreach (TItemCell item in data)
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
                    instance.Initialize(item);
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
    }
}
