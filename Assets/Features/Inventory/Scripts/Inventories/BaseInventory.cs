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

        [Header("Заполнение по-умолчанию"), Space] 
        [SerializeField]
        [Tooltip("Заполнить ли инвентарь дефолтными ячейками (только при активном ограничении количества слотов)")]
        protected bool fillCellsWithDefaults;
        [SerializeField]
        [Tooltip("Предметы для заполнения дефолтных ячеек (оставить путым, если нужны пустые дефолтные ячейки)")]
        protected List<DefaultInventoryItemCell<TData, TType>> defaultItems = new List<DefaultInventoryItemCell<TData, TType>>();
        
        [NonSerialized]
        private int defaultFillPointer;
        
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
        /// <param name="itemId">Идентификатор предмета в ячейке</param>
        /// <param name="itemTypeId">Тип предмета в ячейке</param>
        /// <param name="amount">Количество предмета в ячейке</param>
        /// <param name="autoMerge">Слияние количества, если предметы одинаковые</param>
        /// <param name="targetIndex">Положение по индексу новой ячейки (-1 если в конец)</param>
        /// <returns>Количество не вместившихся предметов</returns>
        public int AddItem(string itemId, string itemTypeId, int amount, bool autoMerge = true, int targetIndex = -1)
        {
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(itemTypeId))
            {
                ServiceDebug.LogWarning($"{name}: невалидный(е) id, добавление отменено");
                return amount;
            }
            if (amount <= 0)
            {
                ServiceDebug.LogWarning($"{name}: невалидное количество ({amount}), добавление отменено");
                return 0;
            }

            EnsureLoaded();

            if (!IsAllowedType(itemTypeId))
            {
                return amount;
            }

            int remaining = amount;
            if (autoMerge)
            {
                // Мердж в существующие стаки
                for (int i = 0; i < data.Count && remaining > 0; i++)
                {
                    TItemCell presentCell = data[i];
                    if (presentCell is not { IsDefault: false }) continue;

                    if (!presentCell.IsSame(itemId, itemTypeId)) continue;
                    if (presentCell.ItemAmount >= BaseItemCell<TData, TType>.MAX_AMOUNT) continue;

                    remaining = presentCell.IncreaseAmount(remaining);
                    onCellUpdated?.Invoke(presentCell.Id);
                }
            }

            // Если существующие стаки заполнены, то создание новых ячеек
            while (remaining > 0)
            {
                int chunk = Mathf.Min(BaseItemCell<TData, TType>.MAX_AMOUNT, remaining);

                EnsureSlotForNewCell();
                if (IsSlotsLimited() && data.Count >= slotsCount) break;

                TItemCell newCell = CreateCell(itemId, itemTypeId, chunk);
                if (newCell == null)
                {
                    ServiceDebug.LogError($"{name}: создание ячейки завершено ошибкой, добавление прервано");
                    break;
                }
                
                Add(newCell, targetIndex);
                remaining -= chunk;
                onCellAdded?.Invoke(newCell.Id);
            }

            FillDefaultSlotsIfNeeded();
            return remaining;
        }

        /// <summary>
        /// Добавить новую ячейку с предметом (с мерджем в существущие ячейки)
        /// </summary>
        /// <param name="itemCell">Новая ячейка с данными предмета</param>
        /// <returns>Количество не вместившихся предметов</returns>
        public int AddItem(TItemCell itemCell, bool autoMerge = true, int targetIndex = -1)
        {
            if (itemCell == null)
            {
                ServiceDebug.LogWarning($"{name}: попытка добавить пустую ячейку, добавление отменено");
                return 0;
            }

            return AddItem(itemCell.ItemId, itemCell.ItemTypeId, itemCell.ItemAmount, autoMerge, targetIndex);
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
            if (data[index].IsEmpty || data[index].IsDefault) return false; 

            if (!Remove(index)) return false;

            onCellRemoved?.Invoke(index);
            FillDefaultSlotsIfNeeded();
            return true;
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

            int index = GetIndexById(itemCellId);
            if (index < 0)
            {
                ServiceDebug.LogWarning($"{name}: ячейка с id «{itemCellId}» не найдена, удаление отменено");
                return false;
            }

            if (!RemoveItem(index))
            {
                return false;
            }

            FillDefaultSlotsIfNeeded();
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
        /// Переместить ячейку в другой инвентарь
        /// </summary>
        /// <param name="cellId">Идентификатор перемещаемой ячейки</param>
        /// <param name="targetInventory">Другой инвентарь, в который происходит перемещение из этого</param>
        /// <returns>true если перемещение успешно, иначе false</returns>
        public bool MoveItem(string cellId, BaseInventory<TItemCell, TData, TType> targetInventory)
        {
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
            if (targetInventory == null)
            {
                ServiceDebug.LogWarning($"{name}: целевой инвентарь не задан, перемещение отменено");
                return false;
            }
            
            if (cell.IsEmpty || cell.IsDefault) return false;

            int index = GetIndexById(cellId);
            int remaining = targetInventory.AddItem(cell.ItemId, cell.ItemTypeId, cell.ItemAmount);
            int movedAmount = cell.ItemAmount - remaining;

            if (movedAmount <= 0) return false;

            if (remaining <= 0)
            {
                RemoveItem(cell);
                onCellMoved?.Invoke(index);

                FillDefaultSlotsIfNeeded();
                return true;
            }
            else
            {
                int residue = cell.DecreaseAmount(movedAmount);
                if (residue > 0)
                {
                    onCellUpdated?.Invoke(cell.Id);
                    return false;
                }

                onCellUpdated?.Invoke(cell.Id);
                onCellMoved?.Invoke(index);

                FillDefaultSlotsIfNeeded();
                return true;
            }
        }

        /// <summary>
        /// Переместить ячейку в другой инвентарь в определенное место
        /// </summary>
        /// <param name="cellId">Идентификатор перемещаемой ячейки</param>
        /// <param name="targetInventory">Другой инвентарь, в который происходит перемещение из этого инвентаря</param>
        /// <param name="targetCellId">Ячейка другого инвентаря, в которую происходит перемещение из этой ячейки</param>
        /// <returns>true если перемещение успешно, иначе false</returns>
        public bool MoveItem(string cellId, BaseInventory<TItemCell, TData, TType> targetInventory, string targetCellId)
        {
            if (string.IsNullOrEmpty(cellId) || string.IsNullOrEmpty(targetCellId))
            {
                ServiceDebug.LogWarning($"{name}: id пуст, перемещение отменено");
                return false;
            }
            if ( (!GetById(cellId, out TItemCell thisCell) || thisCell == null) ||
                 (!GetById(targetCellId, out TItemCell thatCell) || thatCell == null) )
            {
                ServiceDebug.LogWarning($"{name}: ячейка с id «{cellId}» не найдена, перемещение отменено");
                return false;
            }
            if (targetInventory == null)
            {
                ServiceDebug.LogWarning($"{name}: целевой инвентарь не задан, перемещение отменено");
                return false;
            }
            
            if (thisCell.IsEmpty || thisCell.IsDefault) return false;

            int remaining = 0;
            int targetCellIndex = targetInventory.GetIndexById(targetCellId);
            int thisCellIndex = GetIndexById(cellId);
            TItemCell targetCell = targetInventory.GetById(targetCellId);
            
            // Та ячейка пустая/дефолтная — переместить полностью
            if (thatCell.IsDefault || thatCell.IsEmpty)
            { 
                remaining = targetInventory.AddItem(thisCell, false, targetCellIndex);
            }
            // Та ячейка такая же — увеличить количество
            else if (thatCell.IsSame(thisCell)) 
            {
                remaining = targetCell.IncreaseAmount(thisCell.ItemAmount);
            }
            // Та ячейка содержит другой предмет — поменять предметы местами
            else
            {
                targetInventory.RemoveItem(targetCell);
                remaining = targetInventory.AddItem(thisCell, false, targetCellIndex);
                
                RemoveItem(thisCell);
                AddItem(thatCell, false, thisCellIndex);
            }
            
            int movedAmount = thisCell.ItemAmount - remaining;
            if (movedAmount <= 0) return false;

            // Все удалось переместить — удалить ячейку
            if (remaining <= 0)
            {
                RemoveItem(thisCell);
                onCellMoved?.Invoke(thisCellIndex);

                FillDefaultSlotsIfNeeded();
                return true;
            }
            // Не все удалось переместить (осталось количество) — убавить перемещенное количество
            else
            {
                int residue = thisCell.DecreaseAmount(movedAmount);
                if (residue > 0)
                {
                    onCellUpdated?.Invoke(thisCell.Id);
                    return false;
                }

                onCellUpdated?.Invoke(thisCell.Id);
                onCellMoved?.Invoke(thisCellIndex);

                FillDefaultSlotsIfNeeded();
                return true;
            }
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
            
            if (cell.IsEmpty || cell.IsDefault) return false;
            
            if (cell.ItemAmount <= splitAmount)
            {
                ServiceDebug.LogWarning($"{name}: в ячейке {cell.ItemAmount} из {splitAmount} запрашиваемых, разделение отменено");
                return false;
            }

            if (IsSlotsLimited() && data.Count >= slotsCount && GetFirstDefaultCellIndex() < 0)
            {
                ServiceDebug.LogWarning($"{name}: нет слота для новой ячейки (slotsCount={slotsCount}), разделение отменено");
                return false;
            }

            int residue = cell.DecreaseAmount(splitAmount);
            if (residue > 0)
            {
                ServiceDebug.LogWarning($"{name}: разделение выполнено некорректно, остаток удаления = {residue}");
                return false;
            }

            EnsureSlotForNewCell();

            TItemCell newCell = CreateCell(cell.ItemId, cell.ItemTypeId, splitAmount);
            if (newCell == null)
            {
                ServiceDebug.LogError($"{name}: создание ячейки завершено ошибкой, разделение отменено");
                return false;
            }

            Add(newCell);

            onCellSplitted?.Invoke(itemCellId, newCell.Id);

            FillDefaultSlotsIfNeeded();
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
            if (firstItemCellIndex == secondItemCellIndex) return false;

            if (firstItemCellIndex < 0 || secondItemCellIndex < 0)
            {
                ServiceDebug.LogWarning($"{name}: одна из ячеек не найдена, обмен отменён");
                return false;
            }
            if (firstItemCellIndex >= data.Count || secondItemCellIndex >= data.Count)
            {
                ServiceDebug.LogError($"{name}: невалидный(е) индекс(ы), обмен отменён");
                return false;
            }

            (data[firstItemCellIndex], data[secondItemCellIndex]) = (data[secondItemCellIndex], data[firstItemCellIndex]);

            OnDataUpdate();
            MarkDirty();

            onCellSwapped?.Invoke(GetIdByIndex(firstItemCellIndex), GetIdByIndex(secondItemCellIndex));
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

            FillDefaultSlotsIfNeeded();
            return true;
        }
        
        #endregion

        #region Internal
        
        #region Проверки
        
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
        /// Ограничено ли количество слотов в данном инвентаре
        /// </summary>
        private bool IsSlotsLimited() => slotsCount != INFINITE_CELLS_SIGN;
        
        /// <summary>
        /// Разрешен ли тип предмета в данном инвентаре
        /// </summary>
        private bool IsAllowedType(string itemTypeId)
        {
            if (allowedItemType == null) return true;

            if (typeRegistry == null)
            {
                ServiceDebug.LogError($"{name}: задано ограничение типа, но не назначен реестр типов");
                return false;
            }

            TType type = typeRegistry.GetById(itemTypeId);
            if (type == null) return false;

            return type.Id == allowedItemType.Id;
        }
        
        #endregion

        #region Id & Index ячеек
        
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

        #region Манипуляции ячейками

        /// <summary>
        /// Создать новую заполненную ячейку
        /// </summary>
        protected abstract TItemCell CreateCell(string itemId, string itemTypeId, int amount, bool isDefault = false);
        /// <summary>
        /// Создать новую пустую ячейку
        /// </summary>
        protected abstract TItemCell CreateEmptyCell();

        /// <summary>
        /// Удаляет одну дефолтную ячейку, чтобы освободить место для новой.
        /// </summary>
        private int EnsureSlotForNewCell()
        {
            if (!IsSlotsLimited()) return -1;
            if (data.Count < slotsCount) return -1;

            int firstDefaultCellIndex = GetFirstDefaultCellIndex();
            if (firstDefaultCellIndex < 0) return -1;

            Remove(firstDefaultCellIndex);
            return firstDefaultCellIndex;
        }
        
        #endregion

        #region Дефолтные ячейки

        /// <summary>
        /// Если слоты ограничены и текущих ячеек меньше максимума, дозаполняет инвентарь дефолтными ячейками
        /// </summary>
        private void FillDefaultSlotsIfNeeded()
        {
            Debug.Log("FillDefaultSlotsIfNeeded");
            if (!fillCellsWithDefaults || !IsSlotsLimited() || defaultItems == null || defaultItems.Count == 0) return;

            Debug.Log("fillCellsWithDefaults");
            EnsureLoaded();

            while (data.Count < slotsCount)
            {
                Debug.Log("data.Count < slotsCount");
                TItemCell newCell;
                
                if (GetNextDefaultItem(out DefaultInventoryItemCell<TData, TType> nextDefault) == false)
                {
                    newCell = CreateEmptyCell();
                }
                else
                {
                    Debug.Log("GetNextDefaultItem");
                    string defaultItemId = nextDefault.ItemData.Id;
                    string defaultItemTypeId = nextDefault.ItemTypeData.Id;

                    if (string.IsNullOrEmpty(defaultItemId) || string.IsNullOrEmpty(defaultItemTypeId))
                    {
                        ServiceDebug.LogWarning($"{name}: дефолтный предмет содержит невалидный(е) id, заполнение прервано");
                        return;
                    }
                    if (!IsAllowedType(defaultItemTypeId))
                    {
                        defaultFillPointer++;
                        continue;
                    }
                    
                    newCell = CreateCell(defaultItemId, defaultItemTypeId, 1, true);
                }

                Add(newCell);
                Debug.Log("Add");
                onCellAdded?.Invoke(newCell.Id);
            }
        }

        /// <summary>
        /// Возвращает следующий дефолтный предмет по кругу, используя defaultFillPointer
        /// </summary>
        private bool GetNextDefaultItem(out DefaultInventoryItemCell<TData, TType> defaultItem)
        {
            defaultItem = new DefaultInventoryItemCell<TData, TType>();
            
            if (defaultItems == null || defaultItems.Count == 0)
            {
                return false;
            }

            int attempts = 0;

            while (attempts < defaultItems.Count)
            {
                int index = defaultFillPointer % defaultItems.Count;
                defaultFillPointer++;

                DefaultInventoryItemCell<TData, TType> item = defaultItems[index];
                if (!item.IsNull())
                {
                    defaultItem = item;
                    return true;
                }

                attempts++;
            }

            return false;
        }

        /// <summary>
        /// Находит индекс любой дефолтной ячейки в инвентаре
        /// </summary>
        /// <remarks>
        /// Используется для вытеснения дефолта, если нужно освободить слот для недефолтного предмета
        /// </remarks>
        /// <returns>
        /// Индекс первой попавшейся дефолтной ячейки
        /// </returns>
        private int GetFirstDefaultCellIndex()
        {
            for (int i = 0; i < data.Count; i++)
            {
                TItemCell cell = data[i];
                if (cell == null) continue;

                if (cell.IsDefault)
                {
                    return i;
                }
            }

            return -1;
        }
        
        #endregion
        
        #endregion
    }
}