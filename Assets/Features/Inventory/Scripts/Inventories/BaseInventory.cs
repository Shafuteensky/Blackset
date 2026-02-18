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
        /// <param name="BaseInventory">Инвентарь второй ячейки</param>
        public event Action<string, string, BaseInventory<TItemCell, TData, TType>> onCellSwapped;
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
            if (!isReferencesValid(itemCell, dataRegistry)) return null;
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
            if (!isReferencesValid(itemCell, typeRegistry)) return null;
            return itemCell.GetTypeData(typeRegistry);
        }
        
        /// <summary>
        /// Получить данные типа предмета конкретной ячейки по идентификатору ячейки
        /// </summary>
        /// <param name="item">Ячейка предмета</param>
        public TType GetCellTypeData(string cellId)
        {
            if (string.IsNullOrEmpty(cellId))
            {
                ServiceDebug.LogError($"{name}: невалидный id, данные не получены");
                return null;
            }

            TItemCell itemCell = GetById(cellId);
            TType itemTypeData = GetCellTypeData(itemCell);
            
            return itemTypeData;
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
        /// <param name="targetIndex">Положение по индексу новой ячейки (-1 если в конец или первую пустую/дефолтную ячейку)</param>
        /// <returns>Количество не вместившихся предметов (0 если операция полностью успешна)</returns>
        public int AddItem(string itemId, string itemTypeId, int amount, bool autoMerge = true, int targetIndex = -1)
        {
            if ( !checkId(itemId) || 
                 !checkId(itemTypeId) || 
                 !checkAmount(amount) ) return amount;
            EnsureLoaded();
            if (!isAllowedType(itemTypeId)) return amount;

            int remaining = amount;
            
            // Мердж в существующие стаки
            if (autoMerge)
            {
                // Индекс не указан — поместить во все схожие ячейки
                int startIndex = 0;
                int endIndex = Data.Count;
                // Индекс указан — попытка помещения только в указанную ячейку
                if (targetIndex > -1 && targetIndex < Data.Count)
                {
                    startIndex = targetIndex;
                    endIndex = targetIndex + 1;
                }
                
                bool cellUpdated = false;
                for (int i = startIndex; i < endIndex && remaining > 0; i++)
                {
                    TItemCell presentCell = Data[i];
                    if (presentCell is not { IsDefault: false }) continue;

                    if (!presentCell.IsContentSame(itemId, itemTypeId)) continue;
                    if (presentCell.ItemAmount >= BaseItemCell<TData, TType>.MAX_AMOUNT) continue;

                    remaining = presentCell.IncreaseAmount(remaining);
                    onCellUpdated?.Invoke(presentCell.Id);
                    cellUpdated = true;
                }
                if (cellUpdated) MarkDirty();
            }

            // Если существующие стаки заполнены, то создание новых ячеек
            while (remaining > 0)
            {
                int chunk = Mathf.Min(BaseItemCell<TData, TType>.MAX_AMOUNT, remaining);

                int vacantCellIndex = EnsureSlotForNewCell();
                if (isSlotsLimited() && Data.Count >= slotsCount) break;

                TItemCell newCell = CreateCell(itemId, itemTypeId, chunk);
                if (newCell == null)
                {
                    ServiceDebug.LogError($"{name}: создание ячейки завершено ошибкой, добавление прервано");
                    break;
                }

                if (targetIndex == -1) targetIndex = vacantCellIndex;
                Add(newCell, targetIndex); // MarkDirty происходит внутри
                remaining -= chunk;
                onCellAdded?.Invoke(newCell.Id);
            }

            FillDefaultSlotsIfNeeded();
            return remaining;
        }

        /// <summary>
        /// Добавить новую ячейку с предметом (с мерджем в существущие ячейки)
        /// </summary>
        /// <param name="itemCell">Ячейка с данными помещаемого в эту ячейку предмета</param>
        /// <param name="autoMerge">Слияние количества, если предметы одинаковые</param>
        /// <param name="targetIndex">Положение по индексу новой ячейки (-1 если в конец или первую пустую/дефолтную ячейку)</param>
        /// <returns>Количество не вместившихся предметов (0 если операция полностью успешна)</returns>
        public int AddItem(TItemCell itemCell, bool autoMerge = true, int targetIndex = -1)
        {
            if ( !checkCell(itemCell) ) return 0;

            return AddItem(itemCell.ItemId, itemCell.ItemTypeId, itemCell.ItemAmount, autoMerge, targetIndex);
        }
        
        /// <summary>
        /// Удалить существующую ячейку по индексу
        /// </summary>
        /// <param name="itemCell">Удаляемая ячейка</param>
        /// <param name="amount">Количество, если надо удалить не всю ячейку</param>
        /// <returns>Количество не удаленных предметов (0 если операция полностью успешна)</returns>
        public int RemoveItem(int index, int amount = -1)
        {
            if ( !checkIndex(index) || 
                 ( amount != -1 && !checkAmount(amount) ) ) return amount;
            if (Data[index].IsEmpty || 
                Data[index].IsDefault) return amount; 

            if (amount == -1)
            {
                if (!Remove(index)) return amount;
                AddDefaultSlotIfNeeded(index);
                onCellRemoved?.Invoke(index);
            }
            else
            {
                TItemCell itemCell = Data[index];
                int residue = itemCell.DecreaseAmount(amount);
                if (residue == 0)
                {
                    if (!Remove(index)) return amount; // MarkDirty происходит внутри
                    AddDefaultSlotIfNeeded(index);
                    onCellRemoved?.Invoke(index);
                }
                else
                    onCellUpdated?.Invoke(GetIdByIndex(index));
            }

            FillDefaultSlotsIfNeeded();
            return 0;
        }
        
        /// <summary>
        /// Удалить существующую ячейку по идентификатору
        /// </summary>
        /// <param name="itemCellId">Идентификатор ячейки</param>
        /// <param name="amount">Количество, если надо удалить не всю ячейку</param>
        /// <returns>Количество не удаленных предметов (0 если операция полностью успешна)</returns>
        public int RemoveItem(string itemCellId, int amount = -1)
        {
            if ( !checkId(itemCellId) || 
                 ( amount != -1 && !checkAmount(amount) ) ) return amount;
            int index = GetIndexById(itemCellId);
            if ( !checkIndex(index) ) return amount;

            return RemoveItem(index, amount);
        }

        /// <summary>
        /// Удалить существующую ячейку
        /// </summary>
        /// <param name="itemCell">Удаляемая ячейка</param>
        /// <returns>Количество не удаленных предметов (0 если операция полностью успешна)</returns>
        public int RemoveItem(TItemCell itemCell, int amount = -1)
        {
            if ( !checkCell(itemCell) ) return amount;

            return RemoveItem(itemCell.Id, amount);
        }
        
        /// <summary>
        /// Переместить ячейку в другой инвентарь
        /// </summary>
        /// <param name="cellId">Идентификатор перемещаемой ячейки</param>
        /// <param name="targetInventory">Другой инвентарь, в который происходит перемещение из этого</param>
        /// <returns>Количество не перемещенных предметов (0 если операция полностью успешна)</returns>
        public int MoveItem(string cellId, BaseInventory<TItemCell, TData, TType> targetInventory)
        {
            if (targetInventory.Id == Id) return 0;
            if ( !checkId(cellId) || 
                 !GetById(cellId, out TItemCell cell)) return -1;
            if ( !checkCell(cell) || 
                 !checkInventory(targetInventory) ) return cell.ItemAmount;
            if ( targetInventory.allowedItemType != null && 
                 GetById(cellId).ItemTypeId != targetInventory.allowedItemType.Id ) return cell.ItemAmount;
            
            if (cell.IsEmpty || cell.IsDefault) return -1;

            int index = GetIndexById(cellId);
            int remaining = targetInventory.AddItem(cell.ItemId, cell.ItemTypeId, cell.ItemAmount);
            int movedAmount = cell.ItemAmount - remaining;

            if (movedAmount <= 0) return cell.ItemAmount;

            // Все удалось переместить — удалить ячейку
            if (remaining <= 0)
            {
                RemoveItem(cell);
                onCellMoved?.Invoke(index);
            }
            // Не все удалось переместить (осталось количество) — убавить перемещенное количество
            else
            {
                int residue = cell.DecreaseAmount(movedAmount);
                
                onCellUpdated?.Invoke(cell.Id);
                MarkDirty();
                
                if (residue > 0) return residue;
            }
            
            FillDefaultSlotsIfNeeded();
            MarkDirty();
            return 0;
        }

        /// <summary>
        /// Переместить ячейку в другой инвентарь в определенное место
        /// </summary>
        /// <param name="cellId">Идентификатор перемещаемой ячейки</param>
        /// <param name="targetInventory">Другой инвентарь, в который происходит перемещение из этого инвентаря</param>
        /// <param name="targetCellId">Ячейка другого инвентаря, в которую происходит перемещение из этой ячейки</param>
        /// <returns>Количество не перемещенных предметов (0 если операция полностью успешна)</returns>
        public int MoveItem(string cellId, BaseInventory<TItemCell, TData, TType> targetInventory, string targetCellId)
        {
            if ( ReferenceEquals(targetInventory, this) ) return 0;
            if ( !checkId(cellId) || 
                 !checkId(targetCellId) || 
                 !checkInventory(targetInventory) ) return -1;
            if ( !GetById(cellId, out TItemCell thisCell) || 
                 !checkCell(thisCell) ) return -1;
            if ( !targetInventory.GetById(targetCellId, out TItemCell thatCell ) || 
                 !checkCell(thatCell)) return thisCell.ItemAmount;

            if ( thisCell.IsEmpty || 
                 thisCell.IsDefault ) return -1;
            if ( targetInventory.allowedItemType != null && 
                 thisCell.ItemTypeId != targetInventory.allowedItemType.Id ) return thisCell.ItemAmount;

            int thisCellIndex = GetIndexById(cellId);
            int targetCellIndex = targetInventory.GetIndexById(targetCellId);
            if ( !checkIndex(thisCellIndex) || 
                 !checkIndex(targetCellIndex, targetInventory.Data.Count) ) return thisCell.ItemAmount;

            int ApplyMoveResult(int remaining)
            {
                int movedAmount = thisCell.ItemAmount - remaining;
                if (movedAmount <= 0) return thisCell.ItemAmount;

                // Перемещено все — удалить оригинальную ячейку
                if (remaining <= 0) RemoveItem(thisCell);
                // Перемещено не все — убавить перемещенное количество
                else
                {
                    int residue = thisCell.DecreaseAmount(movedAmount);
                    onCellUpdated?.Invoke(thisCell.Id);
                    MarkDirty();
                    if (residue > 0) return residue;
                }

                onCellMoved?.Invoke(thisCellIndex);
                FillDefaultSlotsIfNeeded();
                MarkDirty();

                return 0;
            }

            // Перемещение в дефолтную или пустую ячейку — заменить ее на нужную ячейку
            if (thatCell.IsDefault || thatCell.IsEmpty)
            {
                int remaining = targetInventory.AddItem(thisCell, false, targetCellIndex);
                return ApplyMoveResult(remaining);
            }
            // Перемещение в ячейку с таким же содержимым — увеличить количество
            if (thatCell.IsContentSame(thisCell))
            {
                int before = thatCell.ItemAmount;
                int remaining = thatCell.IncreaseAmount(thisCell.ItemAmount);

                if (thatCell.ItemAmount != before)
                {
                    targetInventory.onCellUpdated?.Invoke(thatCell.Id);
                    targetInventory.MarkDirty();
                }

                return ApplyMoveResult(remaining);
            }
            // Перемещение в заполненную иным содержимым ячейку — поменять ячейки местами
            if (allowedItemType != null && thatCell.ItemTypeId != allowedItemType.Id) return thisCell.ItemAmount;
            if (!SwapItem(thisCellIndex, targetCellIndex, targetInventory)) return thisCell.ItemAmount;

            targetInventory.FillDefaultSlotsIfNeeded();
            targetInventory.MarkDirty();

            return 0;
        }
        
        /// <summary>
        /// Разделение стаков существующей ячейки
        /// </summary>
        /// <param name="itemCellId">Идентификатор ячейки</param>
        /// <param name="splitAmount">Количество для разделения (создания новой ячейки с этим количеством)</param>
        /// <returns>true если разделение успешно, иначе false</returns>
        public bool SplitItem(string itemCellId, int splitAmount)
        {
            if ( !checkId(itemCellId) || !checkAmount(splitAmount) ) return false;
            bool isCellPresent = GetById(itemCellId, out TItemCell cell);
            if ( !isCellPresent || !checkCell(cell) ) return false;
            
            if (cell.IsEmpty || cell.IsDefault) return false;
            
            if (cell.ItemAmount <= splitAmount)
            {
                ServiceDebug.LogWarning($"{name}: в ячейке {cell.ItemAmount} из {splitAmount} запрашиваемых, разделение отменено");
                return false;
            }
            if (isSlotsLimited() && Data.Count >= slotsCount && GetFirstDefaultCellIndex() < 0)
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

            int vacantCellIndex = EnsureSlotForNewCell();

            TItemCell newCell = CreateCell(cell.ItemId, cell.ItemTypeId, splitAmount);
            if (newCell == null)
            {
                ServiceDebug.LogError($"{name}: создание ячейки завершено ошибкой, разделение отменено");
                return false;
            }

            Add(newCell, vacantCellIndex);

            onCellSplitted?.Invoke(itemCellId, newCell.Id);
            onCellUpdated?.Invoke(newCell.Id);
            onCellUpdated?.Invoke(cell.Id);

            FillDefaultSlotsIfNeeded();
            MarkDirty();
            return true;
        }

        /// <summary>
        /// Поменять ячейки местами
        /// </summary>
        /// <param name="firstItemCellIndex">Индекс первой ячейки</param>
        /// <param name="secondItemCellIndex">Индекс второй ячейки</param>
        /// <returns>true если обмен успешен, иначе false</returns>
        public bool SwapItem(int firstItemCellIndex, int secondItemCellIndex, BaseInventory<TItemCell, TData, TType> targetInventory = null)
        {
            if ( firstItemCellIndex == secondItemCellIndex ) return false;
            if ( targetInventory == null ) targetInventory = this;
            if ( !checkIndex(firstItemCellIndex) || 
                 !checkIndex(secondItemCellIndex, targetInventory.Data.Count) ) return false;

            (Data[firstItemCellIndex], targetInventory.Data[secondItemCellIndex]) = 
                (targetInventory.Data[secondItemCellIndex], Data[firstItemCellIndex]);

            FillDefaultSlotsIfNeeded();
            
            string firstCellId = GetIdByIndex(firstItemCellIndex);
            string secondCellId = targetInventory.GetIdByIndex(secondItemCellIndex);
            onCellSwapped?.Invoke(firstCellId, secondCellId, targetInventory);
            onCellUpdated?.Invoke(firstCellId);
            targetInventory.onCellUpdated?.Invoke(secondCellId);
            
            MarkDirty();
            return true;
        }
        
        /// <summary>
        /// Поменять ячейки местами
        /// </summary>
        /// <param name="firstItemCellId">Идентификатор первой ячейки</param>
        /// <param name="secondItemCellId">Идентификатор второй ячейки</param>
        /// <returns>true если обмен успешен, иначе false</returns>
        public bool SwapItem(string firstItemCellId, string secondItemCellId, BaseInventory<TItemCell, TData, TType> targetInventory = null)
        {
            if (targetInventory == null) targetInventory = this;
            if ( !checkInventory(targetInventory) || !checkId(firstItemCellId) || !checkId(secondItemCellId)) return false;
            if (firstItemCellId == secondItemCellId) return false;

            EnsureLoaded();

            int firstIndex = GetIndexById(firstItemCellId);
            int secondIndex = targetInventory.GetIndexById(secondItemCellId);

            if (!SwapItem(firstIndex, secondIndex, targetInventory)) return false;

            return true;
        }
        
        #endregion

        #region Id & Index ячеек
        
        /// <summary>
        /// Получить текущее значение индекса конкретной ячейки по ее идентификатору
        /// </summary>
        public int GetIndexById(string cellId)
        {
            for (int i = 0; i < Data.Count; i++)
            {
                TItemCell cell = Data[i];
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
        public string GetIdByIndex(int itemIndex) => Data[itemIndex].Id;

        #endregion
        
        #region Internal
        
        #region Проверки
        
        /// <summary>
        /// Проверка на null ячейки и реестра
        /// </summary>
        private bool isReferencesValid<TRegistryData>(TItemCell itemCell, BaseDataRegistry<TRegistryData> registry)
            where TRegistryData : BaseData
        {
            if (itemCell == null)
            {
                ServiceDebug.LogError($"{name}: получены невалидные данные ячейки, данные не найдены");
                return false;
            }

            if (registry != null) return true;
            ServiceDebug.LogError($"{name}: реестр данных не назначен, данные не найдены");
            return false;

        }
        
        /// <summary>
        /// Ограничено ли количество слотов в данном инвентаре
        /// </summary>
        private bool isSlotsLimited() => slotsCount != INFINITE_CELLS_SIGN;
        
        /// <summary>
        /// Разрешен ли тип предмета в данном инвентаре
        /// </summary>
        private bool isAllowedType(string itemTypeId)
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

        [HideInCallstack]
        private bool checkIndex(int index, int count = -1)
        {
            if (count == -1) count = Data.Count;
            if (index >= 0 && index < count) return true;
            ServiceDebug.LogError($"{name}: невалидный индекс ({index}), операция отменена");
            return false;
        }

        [HideInCallstack]
        private bool checkId(string id)
        {
            if (!string.IsNullOrEmpty(id)) return true;
            ServiceDebug.LogError($"{name}: невалидный(е) id, операция отменена");
            return false;
        }

        [HideInCallstack]
        private bool checkCell(TItemCell cell)
        {
            if (cell != null) return true;
            ServiceDebug.LogError($"{name}: ячейка невалидна, операция отменена");
            return false;
        }

        [HideInCallstack]
        private bool checkData(TData data)
        {
            if (data != null) return true;
            ServiceDebug.LogError($"{name}: ячейка невалидна, операция отменена");
            return false;
        }

        [HideInCallstack]
        private bool checkInventory(BaseInventory<TItemCell, TData, TType> inventory)
        {
            if (inventory == null)
            {
                ServiceDebug.LogError($"{name}: ссылка на инвентарь невалидена, операция отменена");
                return false;
            }

            if (inventory.Data != null) return true;
            ServiceDebug.LogError($"{name}: ошибка данных инвентаря, операция отменена");
            return false;
        }

        [HideInCallstack]
        private bool checkAmount(int amount)
        {
            if (amount > 0) return true;
            ServiceDebug.LogError($"{name}: невалидное количество ({amount}), операция отменена");
            return false;
        }
        
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
        /// <returns>
        /// Индекс удаленной ячейку
        /// </returns>
        private int EnsureSlotForNewCell()
        {
            if (!isSlotsLimited()) return -1;
            if (Data.Count < slotsCount) return -1;

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
            if (!IsDefaultsNeeded()) return;

            while (Data.Count < slotsCount)
            {
                AddDefaultSlot();
            }
        }

        private void AddDefaultSlotIfNeeded(int index = -1)
        {
            if (!IsDefaultsNeeded()) return;
            AddDefaultSlot(index);
        }
        
        private bool AddDefaultSlot(int index = -1)
        {
            TItemCell newCell;
                
            if (GetNextDefaultItem(out DefaultInventoryItemCell<TData, TType> nextDefault) == false)
            {
                newCell = CreateEmptyCell();
            }
            else
            {
                string defaultItemId = nextDefault.ItemData.Id;
                string defaultItemTypeId = nextDefault.ItemTypeData.Id;

                if (string.IsNullOrEmpty(defaultItemId) || string.IsNullOrEmpty(defaultItemTypeId))
                {
                    ServiceDebug.LogWarning($"{name}: дефолтный предмет содержит невалидный(е) id, заполнение прервано");
                    return false;
                }
                if (!isAllowedType(defaultItemTypeId))
                {
                    defaultFillPointer++;
                    return false;
                }
                    
                newCell = CreateCell(defaultItemId, defaultItemTypeId, 1, true);
            }

            Add(newCell, index);
            onCellAdded?.Invoke(newCell.Id);
            return true;
        }

        private bool IsDefaultsNeeded()
        {
            if (!fillCellsWithDefaults || !isSlotsLimited() || defaultItems == null 
                || (slotsCount > 0 && Data.Count >= slotsCount)) return false;
            return true;
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
            for (int i = 0; i < Data.Count; i++)
            {
                TItemCell cell = Data[i];
                if (cell == null) continue;

                if (cell.IsDefault)
                {
                    return i;
                }
            }

            return -1;
        }
        
        #endregion

        protected override void OnDataLoaded()
        {
            base.OnDataLoaded();
            // При первом запуске заполнять дефолтные инвентари
            FillDefaultSlotsIfNeeded();
        }
        
        #endregion
    }
}