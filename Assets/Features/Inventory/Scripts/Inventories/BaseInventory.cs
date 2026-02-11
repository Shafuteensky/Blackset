using System;
using System.Collections.Generic;
using Extensions.Data.InMemoryData;
using UnityEngine;
using Blackset.Data.Registries;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Extensions.Log;

namespace Blackset.Inventory.Inventories
{
    /// <summary>
    /// Базовый инвентарь игрока
    /// </summary>
    /// <typeparam name="TItemCell">Тип предмета ячейки</typeparam>
    /// <typeparam name="TData">Тип данных предмета</typeparam>
    /// <typeparam name="TType">Тип типа предмета</typeparam>
    /// <remarks>
    /// - Конвенционное редактирование содержимого: при изменение через Data события данных не вызываются
    /// - Максимум вместимости определяется ячейкой // TODO изменить чтобы предмет или его тип обозначал максимум?
    /// - Инвентарь хранит не просто предметы, а уникальные ячейки с предметами
    /// - Инвентарь является заполненным списком: пустые ячейки не существуют (исключение: заполнение дефолтными предметами до максимума вместимости)
    /// </remarks>
    public abstract class BaseInventory<TItemCell, TData, TType> : InMemoryDataContainer<TItemCell>
        where TItemCell : BaseItemCell<TData, TType>
        where TData : BaseData
        where TType : BaseItemType
    {
        private const int INFINITE_CELLS_SIGN = 0;
        
        #region События

        /// <summary>
        /// Событие добавления новой ячейки
        /// </summary>
        /// <param name="string">Идентификатор добавленной ячейки</param>
        public event Action<string> onCellAdded;
        /// <summary>
        /// Событие удаления ячейки
        /// </summary>
        /// <param name="int">Индекс удаленной ячейки</param>
        public event Action<int> onCellRemoved;
        /// <summary>
        /// Событие перемещения ячейки
        /// </summary>
        /// <param name="int">Индекс перемещенной ячейки</param>
        public event Action<int> onCellMoved;
        /// <summary>
        /// Событие разделения ячейки
        /// </summary>
        /// <param name="string">Идентификатор ячейки, убавленной в количестве при раделении</param>
        /// <param name="string">Идентификатор ячейки, созданной при раделении</param>
        public event Action<string, string> onCellSplitted;
        /// <summary>
        /// Событие обмена местами ячеек
        /// </summary>
        /// <param name="string">Идентификатор первой ячейки</param>
        /// <param name="string">Идентификатор второй ячейки</param>
        public event Action<string, string> onCellSwapped;
        /// <summary>
        /// Событие обновления данных ячейки
        /// </summary>
        /// <param name="string">Идентификатор обновленной ячейки</param>
        public event Action<string> onCellUpdated;
        
        #endregion
        
        /// <summary>
        /// Реестр данных предметов для разрешения itemId -> данные
        /// </summary>
        public BaseDataRegistry<TData> DataRegistry => dataRegistry;
        /// <summary>
        /// Реестр данных предметов для разрешения itemId -> данные
        /// </summary>
        public BaseDataRegistry<TType> TypeRegistry => typeRegistry;
        
        /// <summary>
        /// Разрешенный тип предметов (оставить пустым, если без ограничений)
        /// </summary>
        public TType AllowedItemType => allowedItemType;

        [Header("Реестры игровых данных"), Space]
        [SerializeField]
        protected BaseDataRegistry<TData> dataRegistry;
        [SerializeField]
        protected BaseDataRegistry<TType> typeRegistry;

        [Header("Ограничения"), Space]
        [SerializeField]
        [Tooltip("Количество слотов (оставить 0, если без ограничений")]
        [Min(0)]
        protected int slotsCount = INFINITE_CELLS_SIGN;
        [SerializeField]
        [Tooltip("Разрешенный тип предметов (оставить пустым, если без ограничений)")]
        protected TType allowedItemType;
        [SerializeField]
        [Tooltip("Дефолтные предметы, не дающие инвентарю быть пустующим (заполняют пустые ячейки; оставить путым, если заполнение не нужно)")]
        protected List<TData> defaultItemsData = new List<TData>();
        
        #region Получение данных ячеек
        
        /// <summary>
        /// Получить данные предмета конкретной ячейки
        /// </summary>
        /// <param name="itemCell">Ячейка предмета</param>
        public TData GetCellItemData(TItemCell itemCell)
        {
            if (!IsReferencesValid(itemCell, dataRegistry)) return null;
            return itemCell.GetItemData(dataRegistry);
        }
        
        /// <summary>
        /// Получить данные предмета конкретной ячейки по идентификатору ячейки
        /// </summary>
        /// <param name="itemCell">Ячейка предмета</param>
        public TData GetCellItemData(string cellId)
        {
            if (string.IsNullOrEmpty(cellId))
            {
                ServiceDebug.LogError($"{name}: невалидный id, данные не получены");
                return null;
            }

            TItemCell itemCell = GetById(cellId);
            TData itemData = GetCellItemData(itemCell);
            
            return itemData;
        }
        
        /// <summary>
        /// Получить данные типа предмета конкретной ячейки
        /// </summary>
        /// <param name="item">Ячейка предмета</param>
        public TType GetCellTypeData(TItemCell itemCell)
        {
            if (!IsReferencesValid(itemCell, typeRegistry)) return null;
            return itemCell.GetTypeData(typeRegistry);
        }

        #endregion

        #region Манипуляции содержимым инвентаря

        /// <summary>
        /// Добавить новую ячейку с предметом (с мерджем в существущие ячейки)
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemTypeId"></param>
        /// <param name="amount"></param>
        /// <returns>true если добавление успешно, иначе false</returns>
        public bool AddItem(string itemId, string itemTypeId, int amount, bool autoMerge = true)
        {
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(itemTypeId))
            {
                ServiceDebug.LogWarning($"{name}: невалидный(е) id, добавление отменено");
                return false;
            }
            if (amount <= 0)
            {
                ServiceDebug.LogWarning($"{name}: невалидное количество ({amount}), добавление отменено");
                return false;
            }

            EnsureLoaded();

            int remaining = amount;

            if (autoMerge)
            {
                // Мердж в существующие стаки
                for (int i = 0; i < data.Count && remaining > 0; i++)
                {
                    TItemCell cell = data[i];
                    if (!cell.IsSame(itemId, itemTypeId)) continue;
                    if (cell.ItemAmount >= BaseItemCell<TData, TType>.MAX_AMOUNT) continue;

                    remaining = cell.IncreaseAmount(remaining);
                    onCellUpdated?.Invoke(cell.Id);
                }
            }

            // Если существующие стаки заполнены, то создание новых ячеек
            while (remaining > 0)
            {
                int chunk = Mathf.Min(BaseItemCell<TData, TType>.MAX_AMOUNT, remaining);

                TItemCell newCell = CreateCell(itemId, itemTypeId, chunk);
                Add(newCell);

                remaining -= chunk;
                onCellAdded?.Invoke(newCell.Id);
            }
            return true;
        }

        /// <summary>
        /// Добавить новую ячейку с предметом (с мерджем в существущие ячейки)
        /// </summary>
        /// <param name="itemCell">Новая ячейка с данными предмета</param>
        /// <returns>true если добавление успешно, иначе false</returns>
        public bool AddItem(TItemCell itemCell, bool autoMerge = true)
        {
            if (itemCell == null)
            {
                ServiceDebug.LogWarning($"{name}: попытка добавить пустую ячейку, добавление отменено");
                return false;
            }

            return AddItem(itemCell.ItemId, itemCell.ItemTypeId, itemCell.ItemAmount, autoMerge);
        }

        /// <summary>
        /// Удалить существующую ячейку по идентификатору
        /// </summary>
        /// <param name="itemCellId">Идентификатор ячейки</param>
        /// <returns>true если удаление успешно, иначе false</returns>
        public bool RemoveItem(string itemCellId)
        {
            if (string.IsNullOrEmpty(itemCellId))
            {
                ServiceDebug.LogWarning($"{name}: id невалиден, удаление отменено");
                return false;
            }

            if (!GetById(itemCellId, out TItemCell cell) || cell == null)
            {
                ServiceDebug.LogWarning($"{name}: ячейка с id «{itemCellId}» не найдена, удаление отменено");
                return false;
            }

            Remove(cell);
            onCellRemoved?.Invoke(GetIndexById(itemCellId));
            return true;
        }

        /// <summary>
        /// Удалить существующую ячейку
        /// </summary>
        /// <param name="itemCell">Удаляемая ячейка</param>
        /// <returns>true если удаление успешно, иначе false</returns>
        public bool RemoveItem(TItemCell itemCell)
        {
            if (itemCell == null)
            {
                ServiceDebug.LogWarning($"{name}: попытка удалить пустую ячейку, удаление отменено");
                return false;
            }

            return RemoveItem(itemCell.Id);
        }
        
        /// <summary>
        /// Удалить существующую ячейку по индексу
        /// </summary>
        /// <param name="itemCell">Удаляемая ячейка</param>
        /// <returns>true если удаление успешно, иначе false</returns>
        public bool RemoveItem(int index)
        {
            if (index < 0 || index >= data.Count)
            {
                ServiceDebug.LogError($"{name}: невалидный индекс ({index}), удаление отменено");
                return false;
            }

            return RemoveItem(GetIdByIndex(index));
        }
        
        /// <summary>
        /// Переместить ячейку в другой инвентарь
        /// </summary>
        /// <param name="cellId">Идентификатор перемещаемой ячейки</param>
        /// <param name="targetInventory">Другой инвентарь, в который происходит перемещение из этого</param>
        /// <returns>true если перемещение успешно, иначе false</returns>
        public bool MoveItem(string cellId, BaseInventory<TItemCell, TData, TType> targetInventory)
        {
            if (targetInventory == null)
            {
                ServiceDebug.LogWarning($"{name}: целевой инвентарь не задан, перемещение отменено");
                return false;
            }
            if (string.IsNullOrEmpty(cellId))
            {
                ServiceDebug.LogWarning($"{name}: id пуст, перемещение отменено");
                return false;
            }
            if (!GetById(cellId, out TItemCell cell) || cell == null)
            {
                ServiceDebug.LogWarning($"{name}: ячейка с id «{cellId}» не найдена, перемещение отменено");
                return false;
            }

            if (!targetInventory.AddItem(cell.ItemId, cell.ItemTypeId, cell.ItemAmount))
            {
                return false;
            }

            Remove(cell);
            onCellMoved?.Invoke(GetIndexById(cellId));
            return true;
        }
        
        /// <summary>
        /// Разделение стаков существующей ячейки
        /// </summary>
        /// <param name="itemCellId">Идентификатор ячейки</param>
        /// <param name="splitAmount">Количество для разделения (создания новой ячейки с этим количеством)</param>
        /// <returns>true если разделение успешно, иначе false</returns>
        public bool SplitItem(string itemCellId, int splitAmount)
        {
            if (string.IsNullOrEmpty(itemCellId))
            {
                ServiceDebug.LogWarning($"{name}: id пуст, разделение отменено");
                return false;
            }
            if (splitAmount <= 0)
            {
                ServiceDebug.LogWarning($"{name}: количество невалидно ({splitAmount}), разделение отменено");
                return false;
            }
            if (!GetById(itemCellId, out TItemCell cell) || cell == null)
            {
                ServiceDebug.LogWarning($"{name}: ячейка с id «{itemCellId}» не найдена, разделение отменено");
                return false;
            }
            if (cell.ItemAmount <= splitAmount)
            {
                ServiceDebug.LogWarning($"{name}: в ячейке {cell.ItemAmount} из {splitAmount} запрашиваемых, разделение отменено");
                return false;
            }

            int residue = cell.DecreaseAmount(splitAmount);
            if (residue > 0)
            {
                ServiceDebug.LogWarning($"{name}: разделение выполнено некорректно, остаток удаления = {residue}");
                return false;
            }

            TItemCell newCell = CreateCell(cell.ItemId, cell.ItemTypeId, splitAmount);
            if (newCell == null)
            {
                ServiceDebug.LogError($"{name}: создание ячейки завершено ошибкой, разделение отменено");
                return false;
            }

            Add(newCell);
            
            onCellSplitted?.Invoke(itemCellId, newCell.Id);
            return true;
        }
        
        /// <summary>
        /// Поменять ячейки местами
        /// </summary>
        /// <param name="firstItemCellId">Идентификатор первой ячейки</param>
        /// <param name="secondItemCellId">Идентификатор второй ячейки</param>
        /// <returns>true если обмен успешен, иначе false</returns>
        public bool SwapItem(string firstItemCellId, string secondItemCellId)
        {
            if (string.IsNullOrEmpty(firstItemCellId) || string.IsNullOrEmpty(secondItemCellId))
            {
                ServiceDebug.LogWarning($"{name}: невалидный(е) id, обмен отменён");
                return false;
            }

            if (firstItemCellId == secondItemCellId)
            {
                return false;
            }
            
            EnsureLoaded();

            int firstIndex = GetIndexById(firstItemCellId);
            int secondIndex = GetIndexById(secondItemCellId);

            if (!SwapItem(firstIndex, secondIndex)) return false;

            return true;
        }

        /// <summary>
        /// Поменять ячейки местами
        /// </summary>
        /// <param name="firstItemCellIndex">Индекс первой ячейки</param>
        /// <param name="secondItemCellIndex">Индекс второй ячейки</param>
        /// <returns>true если обмен успешен, иначе false</returns>
        public bool SwapItem(int firstItemCellIndex, int secondItemCellIndex)
        {
            if (firstItemCellIndex == secondItemCellIndex)
            {
                return false;
            }

            if (firstItemCellIndex < 0 || secondItemCellIndex < 0)
            {
                ServiceDebug.LogWarning($"{name}: одна из ячеек не найдена, обмен отменён");
                return false;
            }

            (data[firstItemCellIndex], data[secondItemCellIndex]) = (data[secondItemCellIndex], data[firstItemCellIndex]);

            OnDataUpdate();
            MarkDirty();

            onCellSwapped?.Invoke(GetIdByIndex(firstItemCellIndex), GetIdByIndex(secondItemCellIndex));
            return true;
        }
        
        #endregion
        
        #region Internal
        
        /// <summary>
        /// Создать новую ячейку
        /// </summary>
        protected abstract TItemCell CreateCell(string itemId, string itemTypeId, int amount);

        /// <summary>
        /// Проверка на null ячейки и реестра
        /// </summary>
        private bool IsReferencesValid<TRegistryData>(TItemCell itemCell, BaseDataRegistry<TRegistryData> registry)
            where TRegistryData : BaseData
        {
            if (itemCell == null)
            {
                ServiceDebug.LogError($"{name}: получены невалидные данные ячейки, данные не найдены");
                return false;
            }

            if (registry == null)
            {
                ServiceDebug.LogError($"{name}: реестр данных не назначен, данные не найдены");
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Получить текущее значение индекса конкретной ячейки по ее идентификатору
        /// </summary>
        private int GetIndexById(string cellId)
        {
            for (int i = 0; i < data.Count; i++)
            {
                TItemCell cell = data[i];
                if (cell == null) continue;

                if (cell.Id == cellId)
                {
                    return i;
                }
            }

            return -1;
        }
        
        /// <summary>
        /// Получить текущий идентификатор конкретной ячейки по ее индексу
        /// </summary>
        private string GetIdByIndex(int itemIndex) => data[itemIndex].Id;
        
        #endregion
        
    }
}