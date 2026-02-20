using System;
using System.Collections.Generic;
using Extensions.Data.InMemoryData;
using UnityEngine;
using Blackset.Data.Registries;
using Blackset.Data.Items.Types;
using Blackset.Inventories.Cells;
using Extensions.Log;
using Features.Inventory.Scripts.Items;

namespace Blackset.Inventories
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
    public class Inventory : InMemoryDataContainer<InventoryCell>
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
        public event Action<string, string, Inventory> onCellSwapped;
        /// <summary>
        /// Событие обновления данных ячейки
        /// </summary>
        /// <param name="string">Идентификатор обновленной ячейки</param>
        public event Action<string> onCellUpdated;
        
        #endregion
        
        /// <summary>
        /// Реестр данных предметов для разрешения itemId -> данные
        /// </summary>
        public InventoryItemsRegistry DataRegistry => dataRegistry;
        /// <summary>
        /// Реестр данных предметов для разрешения itemId -> данные
        /// </summary>
        public InventoryItemTypesRegistry TypeRegistry => typeRegistry;
        /// <summary>
        /// Максимум предметов в ячейке инвентаря
        /// </summary>
        public int MaxCellAmount => maxCellAmount;
        /// <summary>
        /// Разрешенный тип предметов (оставить пустым, если без ограничений)
        /// </summary>
        public InventoryItemType AllowedItemType => allowedItemType;

        [Header("Реестры игровых данных"), Space]
        [SerializeField]
        protected InventoryItemsRegistry dataRegistry;
        [SerializeField]
        protected InventoryItemTypesRegistry typeRegistry;

        [Header("Ограничения"), Space]
        [SerializeField] 
        [Tooltip("Максимум предметов в ячейке инвентаря")]
        [Range(1, 100)]
        protected int maxCellAmount = 99;
        [SerializeField]
        [Tooltip("Количество слотов (оставить 0, если без ограничений")]
        [Min(0)]
        protected int slotsCount = INFINITE_CELLS_SIGN;
        [SerializeField]
        [Tooltip("Разрешенный класс предметов (оставить пустым, если без ограничений)")]
        protected ItemClass allowedItemClass;
        [SerializeField]
        [Tooltip("Разрешенный тип предметов (оставить пустым, если без ограничений)")]
        protected InventoryItemType allowedItemType;

        [Header("Заполнение по-умолчанию"), Space] 
        [SerializeField]
        [Tooltip("Заполнить ли инвентарь дефолтными ячейками (только при активном ограничении количества слотов)")]
        protected bool fillCellsWithDefaults;
        [SerializeField]
        [Tooltip("Предметы для заполнения дефолтных ячеек (оставить путым, если нужны пустые дефолтные ячейки)")]
        protected List<DefaultInventoryItemCell> defaultItems = new List<DefaultInventoryItemCell>();
        
        [NonSerialized]
        private int defaultFillPointer;
        
        #region Получение данных ячеек
        
        /// <summary>
        /// Получить данные предмета конкретной ячейки
        /// </summary>
        /// <param name="cell">Ячейка предмета</param>
        public InventoryItem GetCellItemData(InventoryCell cell)
        {
            if (!IsReferencesValid(cell, dataRegistry)) return null;
            return cell.GetItemData(dataRegistry);
        }
        
        /// <summary>
        /// Получить данные предмета конкретной ячейки по идентификатору ячейки
        /// </summary>
        /// <param name="itemCell">Ячейка предмета</param>
        public InventoryItem GetCellItemData(string cellId)
        {
            if (string.IsNullOrEmpty(cellId))
            {
                ServiceDebug.LogError($"{name}: невалидный id, данные не получены");
                return null;
            }

            InventoryCell cell = GetById(cellId);
            InventoryItem item = GetCellItemData(cell);
            
            return item;
        }
        
        /// <summary>
        /// Получить данные типа предмета конкретной ячейки
        /// </summary>
        /// <param name="item">Ячейка предмета</param>
        public InventoryItemType GetCellTypeData(InventoryCell cell)
        {
            if (!IsReferencesValid(cell, typeRegistry)) return null;
            return cell.GetTypeData(typeRegistry);
        }
        
        /// <summary>
        /// Получить данные типа предмета конкретной ячейки по идентификатору ячейки
        /// </summary>
        /// <param name="item">Ячейка предмета</param>
        public InventoryItemType GetCellTypeData(string cellId)
        {
            if (string.IsNullOrEmpty(cellId))
            {
                ServiceDebug.LogError($"{name}: невалидный id, данные не получены");
                return null;
            }

            InventoryCell cell = GetById(cellId);
            InventoryItemType itemTypeData = GetCellTypeData(cell);
            
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
            if ( !CheckId(itemId) || 
                 !CheckId(itemTypeId) || 
                 !CheckAmount(amount) ) return amount;
            EnsureLoaded();
            if (!IsItemAllowed(itemId, itemTypeId, this)) return amount;

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
                    InventoryCell presentCell = Data[i];
                    if (presentCell is not { IsDefault: false }) continue;

                    if (!presentCell.IsContentSame(itemId, itemTypeId)) continue;
                    if (presentCell.ItemAmount >= maxCellAmount) continue;

                    remaining = presentCell.IncreaseAmount(remaining, maxCellAmount);
                    onCellUpdated?.Invoke(presentCell.Id);
                    cellUpdated = true;
                }
                if (cellUpdated) MarkDirty();
            }

            // Если существующие стаки заполнены, то создание новых ячеек
            while (remaining > 0)
            {
                int chunk = Mathf.Min(maxCellAmount, remaining);

                int vacantCellIndex = EnsureSlotForNewCell();
                if (IsSlotsLimited() && Data.Count >= slotsCount) break;

                InventoryCell newCell = CreateCell(itemId, itemTypeId, chunk);
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
        /// <param name="cell">Ячейка с данными помещаемого в эту ячейку предмета</param>
        /// <param name="autoMerge">Слияние количества, если предметы одинаковые</param>
        /// <param name="targetIndex">Положение по индексу новой ячейки (-1 если в конец или первую пустую/дефолтную ячейку)</param>
        /// <returns>Количество не вместившихся предметов (0 если операция полностью успешна)</returns>
        public int AddItem(InventoryCell cell, bool autoMerge = true, int targetIndex = -1)
        {
            if ( !CheckCell(cell) ) return 0;

            return AddItem(cell.ItemId, cell.ItemTypeId, cell.ItemAmount, autoMerge, targetIndex);
        }
        
        /// <summary>
        /// Удалить существующую ячейку по индексу
        /// </summary>
        /// <param name="itemCell">Удаляемая ячейка</param>
        /// <param name="amount">Количество, если надо удалить не всю ячейку</param>
        /// <returns>Количество не удаленных предметов (0 если операция полностью успешна)</returns>
        public int RemoveItem(int index, int amount = -1)
        {
            if ( !CheckIndex(index) || 
                 ( amount != -1 && !CheckAmount(amount) ) ) return amount;
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
                InventoryCell cell = Data[index];
                int residue = cell.DecreaseAmount(amount, maxCellAmount);
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
            if ( !CheckId(itemCellId) || 
                 ( amount != -1 && !CheckAmount(amount) ) ) return amount;
            int index = GetIndexById(itemCellId);
            if ( !CheckIndex(index) ) return amount;

            return RemoveItem(index, amount);
        }

        /// <summary>
        /// Удалить существующую ячейку
        /// </summary>
        /// <param name="cell">Удаляемая ячейка</param>
        /// <returns>Количество не удаленных предметов (0 если операция полностью успешна)</returns>
        public int RemoveItem(InventoryCell cell, int amount = -1)
        {
            if ( !CheckCell(cell) ) return amount;

            return RemoveItem(cell.Id, amount);
        }
        
        /// <summary>
        /// Переместить ячейку в другой инвентарь
        /// </summary>
        /// <param name="cellId">Идентификатор перемещаемой ячейки</param>
        /// <param name="targetInventory">Другой инвентарь, в который происходит перемещение из этого</param>
        /// <returns>Количество не перемещенных предметов (0 если операция полностью успешна)</returns>
        public int MoveItem(string cellId, Inventory targetInventory)
        {
            if (targetInventory.Id == Id) return 0;
            if ( !CheckId(cellId) || 
                 !GetById(cellId, out InventoryCell thisCell)) return -1;
            if ( !CheckCell(thisCell) || 
                 !CheckInventory(targetInventory) ) return thisCell.ItemAmount;
            int itemAmount = thisCell.ItemAmount;
            if (!targetInventory.IsItemAllowed(GetById(cellId), this)) return itemAmount;
            
            if (thisCell.IsEmpty || thisCell.IsDefault) return -1;

            int index = GetIndexById(cellId);
            int remaining = targetInventory.AddItem(thisCell.ItemId, thisCell.ItemTypeId, itemAmount);
            int movedAmount = itemAmount - remaining;

            if (movedAmount <= 0) return itemAmount;

            // Все удалось переместить — удалить ячейку
            if (remaining <= 0)
            {
                RemoveItem(thisCell);
                onCellMoved?.Invoke(index);
            }
            // Не все удалось переместить (осталось количество) — убавить перемещенное количество
            else
            {
                int residue = thisCell.DecreaseAmount(movedAmount, maxCellAmount);
                
                onCellUpdated?.Invoke(thisCell.Id);
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
        public int MoveItem(string cellId, Inventory targetInventory, string targetCellId)
        {
            if ( !CheckId(cellId) || 
                 !CheckId(targetCellId) || 
                 !CheckInventory(targetInventory) ) return -1;
            
            if (targetInventory.Id == Id)
            {
                if (cellId == targetCellId) return 0;
                if (!SwapItem(cellId, targetCellId, this)) return -1;
                FillDefaultSlotsIfNeeded();
                MarkDirty();
                return 0;
            }
            
            if ( !GetById(cellId, out InventoryCell thisCell) || 
                 !CheckCell(thisCell) ) return -1;
            if ( !targetInventory.GetById(targetCellId, out InventoryCell thatCell ) || 
                 !CheckCell(thatCell)) return thisCell.ItemAmount;

            if ( thisCell.IsEmpty || 
                 thisCell.IsDefault ) return -1;
            if (!targetInventory.IsItemAllowed(GetById(cellId), this)) return thisCell.ItemAmount;

            int thisCellIndex = GetIndexById(cellId);
            int targetCellIndex = targetInventory.GetIndexById(targetCellId);
            if ( !CheckIndex(thisCellIndex) || 
                 !CheckIndex(targetCellIndex, targetInventory.Data.Count) ) return thisCell.ItemAmount;

            int ApplyMoveResult(int remaining)
            {
                int movedAmount = thisCell.ItemAmount - remaining;
                if (movedAmount <= 0) return thisCell.ItemAmount;

                // Перемещено все — удалить оригинальную ячейку
                if (remaining <= 0) RemoveItem(thisCell);
                // Перемещено не все — убавить перемещенное количество
                else
                {
                    int residue = thisCell.DecreaseAmount(movedAmount, maxCellAmount);
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
                int movedToTarget = Mathf.Min(thisCell.ItemAmount, targetInventory.maxCellAmount);
                if (movedToTarget <= 0) return thisCell.ItemAmount;

                InventoryCell newCell = targetInventory.CreateCell(thisCell.ItemId, thisCell.ItemTypeId, movedToTarget);
                if (newCell == null) return thisCell.ItemAmount;

                targetInventory.Data[targetCellIndex] = newCell;
                targetInventory.onCellRemoved?.Invoke(targetCellIndex);
                targetInventory.onCellAdded?.Invoke(newCell.Id);
                targetInventory.MarkDirty();

                int remaining = thisCell.ItemAmount - movedToTarget;
                return ApplyMoveResult(remaining);
            }
            // Перемещение в ячейку с таким же содержимым — увеличить количество
            if (thatCell.IsContentSame(thisCell))
            {
                int before = thatCell.ItemAmount;
                int remaining = thatCell.IncreaseAmount(thisCell.ItemAmount, targetInventory.maxCellAmount);

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
            if ( !CheckId(itemCellId) || !CheckAmount(splitAmount) ) return false;
            bool isCellPresent = GetById(itemCellId, out InventoryCell cell);
            if ( !isCellPresent || !CheckCell(cell) ) return false;
            
            if (cell.IsEmpty || cell.IsDefault) return false;
            
            if (cell.ItemAmount <= splitAmount)
            {
                ServiceDebug.LogWarning($"{name}: в ячейке {cell.ItemAmount} из {splitAmount} запрашиваемых, разделение отменено");
                return false;
            }
            if (IsSlotsLimited() && Data.Count >= slotsCount && GetFirstDefaultCellIndex() < 0)
            {
                ServiceDebug.LogWarning($"{name}: нет слота для новой ячейки (slotsCount={slotsCount}), разделение отменено");
                return false;
            }
            int residue = cell.DecreaseAmount(splitAmount, maxCellAmount);
            if (residue > 0)
            {
                ServiceDebug.LogWarning($"{name}: разделение выполнено некорректно, остаток удаления = {residue}");
                return false;
            }

            int vacantCellIndex = EnsureSlotForNewCell();

            InventoryCell newCell = CreateCell(cell.ItemId, cell.ItemTypeId, splitAmount);
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
        
        #endregion

        #region Id & Index ячеек
        
        /// <summary>
        /// Получить текущее значение индекса конкретной ячейки по ее идентификатору
        /// </summary>
        public int GetIndexById(string cellId)
        {
            for (int i = 0; i < Data.Count; i++)
            {
                InventoryCell cell = Data[i];
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
        private bool IsReferencesValid(InventoryCell cell, BaseDataRegistry registry)
        {
            if (cell == null)
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
        private bool IsSlotsLimited() => slotsCount != INFINITE_CELLS_SIGN;
        
        /// <summary>
        /// Разрешен ли тип предмета в данном инвентаре
        /// </summary>
        /// <param name="itemClass">Класс предмета</param>
        /// <param name="itemTypeId">Идентификатор типа предмета</param>
        /// <returns>true если класс и тип подходят, иначе false</returns>
        private bool IsItemAllowed(string itemId, string typeId, Inventory fromInventory)
        {
            // Реестры данных различных
            if (fromInventory.dataRegistry.Id != dataRegistry.Id ||
                fromInventory.typeRegistry.Id != typeRegistry.Id) return false;
            
            // Нет ограничений от инвентаря — true
            if (allowedItemClass == ItemClass.Any && allowedItemType == null) return true;
            
            // Класс предмета не совпадает — false
            InventoryItem item = fromInventory.dataRegistry.GetById(itemId);
            if (allowedItemClass != ItemClass.Any && allowedItemClass != item.ItemClass) return false;
            
            // Класс совпадает, ограничений по типу нет — true
            if (allowedItemType == null) return true;
            
            // Тип предмета не совпадает — false
            InventoryItemType type = fromInventory.typeRegistry.GetById(typeId);
            if (allowedItemType != null && allowedItemType != type) return false;
            
            // Класс и тип предмета совпадают или разрешены — true
            return true;
        }

        /// <summary>
        /// Разрешен ли тип предмета в данном инвентаре
        /// </summary>
        /// <param name="InventoryCell">Ячейка инвентаря</param>
        /// <returns>true если класс и тип подходят, иначе false</returns>
        private bool IsItemAllowed(InventoryCell cell, Inventory fromInventory)
        {
            return IsItemAllowed(cell.ItemId, cell.ItemTypeId, fromInventory);
        }
        
        [HideInCallstack]
        private bool CheckIndex(int index, int count = -1)
        {
            if (count == -1) count = Data.Count;
            if (index >= 0 && index < count) return true;
            ServiceDebug.LogError($"{name}: невалидный индекс ({index}), операция отменена");
            return false;
        }

        [HideInCallstack]
        private bool CheckId(string anyId)
        {
            if (!string.IsNullOrEmpty(anyId)) return true;
            ServiceDebug.LogError($"{name}: невалидный(е) id, операция отменена");
            return false;
        }

        [HideInCallstack]
        private bool CheckCell(InventoryCell cell)
        {
            if (cell != null) return true;
            ServiceDebug.LogError($"{name}: ячейка невалидна, операция отменена");
            return false;
        }

        [HideInCallstack]
        private bool CheckInventory(Inventory inventory)
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
        private bool CheckAmount(int amount)
        {
            if (amount > 0) return true;
            ServiceDebug.LogError($"{name}: невалидное количество ({amount}), операция отменена");
            return false;
        }
        
        #endregion

        #region Манипуляции ячейками
        
        protected InventoryCell CreateCell(string itemId, string itemTypeId, int amount, bool isDefault = false)
        {
            if (amount > maxCellAmount) ServiceDebug.LogWarning("Создана ячейка с количеством, больше дозволенного максимума");
            InventoryCell newCell = new InventoryCell(itemId, itemTypeId, amount, isDefault);
            return newCell;
        }
        
        protected InventoryCell CreateEmptyCell()
        {
            InventoryCell newCell = new InventoryCell("", "", 0, true, true);
            return newCell;
        }

        /// <summary>
        /// Удаляет одну дефолтную ячейку, чтобы освободить место для новой.
        /// </summary>
        /// <returns>
        /// Индекс удаленной ячейку
        /// </returns>
        private int EnsureSlotForNewCell()
        {
            if (!IsSlotsLimited()) return -1;
            if (Data.Count < slotsCount) return -1;

            int firstDefaultCellIndex = GetFirstDefaultCellIndex();
            if (firstDefaultCellIndex < 0) return -1;

            Remove(firstDefaultCellIndex);
            return firstDefaultCellIndex;
        }
        
        /// <summary>
        /// Поменять ячейки местами
        /// </summary>
        /// <param name="thisItemCellIndex">Индекс первой ячейки</param>
        /// <param name="targetItemCellIndex">Индекс второй ячейки</param>
        /// <returns>true если обмен успешен, иначе false</returns>
        private bool SwapItem(int thisItemCellIndex, int targetItemCellIndex, Inventory targetInventory = null)
        {
            if ( thisItemCellIndex == targetItemCellIndex ) return false;
            if ( targetInventory == null ) targetInventory = this;
            if ( !CheckIndex(thisItemCellIndex) || 
                 !CheckIndex(targetItemCellIndex, targetInventory.Data.Count) ) return false;
            if (!IsItemAllowed(GetByIndex(thisItemCellIndex), targetInventory)) return false;
            if (!IsItemAllowed(targetInventory.GetByIndex(targetItemCellIndex), targetInventory)) return false;

            (Data[thisItemCellIndex], targetInventory.Data[targetItemCellIndex]) = 
                (targetInventory.Data[targetItemCellIndex], Data[thisItemCellIndex]);

            FillDefaultSlotsIfNeeded();
            
            string firstCellId = GetIdByIndex(thisItemCellIndex);
            string secondCellId = targetInventory.GetIdByIndex(targetItemCellIndex);
            onCellSwapped?.Invoke(firstCellId, secondCellId, targetInventory);
            onCellUpdated?.Invoke(firstCellId);
            targetInventory.onCellUpdated?.Invoke(secondCellId);
            
            MarkDirty();
            return true;
        }
        
        /// <summary>
        /// Поменять ячейки местами
        /// </summary>
        /// <param name="firstItemCellId">Идентификатор первой ячейки (этот инвентарь)</param>
        /// <param name="secondItemCellId">Идентификатор второй ячейки (целевой инвентарь)</param>
        /// <param name="targetInventory">Инвентарь второй ячейки (целевой)</param>
        /// <returns>true если обмен успешен, иначе false</returns>
        private bool SwapItem(string firstItemCellId, string secondItemCellId, Inventory targetInventory = null)
        {
            if (targetInventory == null) targetInventory = this;
            if ( !CheckInventory(targetInventory) || !CheckId(firstItemCellId) || !CheckId(secondItemCellId)) return false;
            if (firstItemCellId == secondItemCellId) return false;

            EnsureLoaded();

            int firstIndex = GetIndexById(firstItemCellId);
            int secondIndex = targetInventory.GetIndexById(secondItemCellId);

            if (!SwapItem(firstIndex, secondIndex, targetInventory)) return false;

            return true;
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
                bool isSlotAdded = AddDefaultSlot();
                if (!isSlotAdded) break;
            }
        }

        private void AddDefaultSlotIfNeeded(int index = -1)
        {
            if (!IsDefaultsNeeded()) return;
            AddDefaultSlot(index);
        }
        
        private bool AddDefaultSlot(int index = -1)
        {
            InventoryCell newCell;
                
            if (GetNextDefaultItem(out DefaultInventoryItemCell nextDefault) == false)
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
                if (!IsItemAllowed(nextDefault.ItemData.Id, defaultItemTypeId, this))
                {
                    ServiceDebug.LogError("Дефолтный предмет не подходит под ограничения инвентаря");
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
            if (!fillCellsWithDefaults || !IsSlotsLimited() || defaultItems == null 
                || (slotsCount > 0 && Data.Count >= slotsCount)) return false;
            return true;
        }

        /// <summary>
        /// Возвращает следующий дефолтный предмет по кругу, используя defaultFillPointer
        /// </summary>
        private bool GetNextDefaultItem(out DefaultInventoryItemCell defaultItem)
        {
            defaultItem = new DefaultInventoryItemCell();
            
            if (defaultItems == null || defaultItems.Count == 0)
            {
                return false;
            }

            int attempts = 0;

            while (attempts < defaultItems.Count)
            {
                int index = defaultFillPointer % defaultItems.Count;
                defaultFillPointer++;

                DefaultInventoryItemCell item = defaultItems[index];
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
                InventoryCell cell = Data[i];
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