using System;
using Blackset.Data.Registries;
using Extensions.Data.InMemoryData;
using UnityEngine;
using Blackset.Data.Items.Types;
using Blackset.Inventories.Items;
using Extensions.Log;

namespace Blackset.Inventories.Cells
{
    /// <summary>
    /// Базовый предмет ячейки инвентаря
    /// </summary>
    /// <typeparam name="TData">Тип данных предмета</typeparam>
    /// <typeparam name="TType">Тип типа предмета</typeparam>
    [Serializable]
    public class InventoryCell : InMemoryDataEntry 
    {
        public const int DEFAULT_MAX_AMOUNT = 99;

        #region События

        /// <summary>
        /// Событие изменения количества
        /// </summary>
        /// <returns>
        /// { id ячейки, новое количество }
        /// </returns>
        public event Action<string, int> onAmountChanged;
        /// <summary>
        /// Событие исчерпания количества предмета в ячейке
        /// </summary>
        /// <returns>
        /// { id ячейки }
        /// </returns>
        public event Action<string> onAmountDepleted;
        /// <summary>
        /// Событие привышения максимума количества предмета в ячейке
        /// </summary>
        /// <returns>
        /// { id ячейки, излишек }
        /// </returns>
        public event Action<string, int> onAmountExceeded;
        
        #endregion
        
        /// <summary>
        /// Пуста ли ячейка
        /// </summary>
        public bool IsEmpty { get => isEmpty; private set => isEmpty = value; }
        /// <summary>
        /// Является ли ячейка дефолтной
        /// </summary>
        public bool IsDefault { get => isDefault; private set => isDefault = value; }
        
        /// <summary>
        /// Идентификатор данных предмета в этой ячейке
        /// </summary>
        public string ItemId { get => itemId; private set => itemId = value; }
        /// <summary>
        /// Идентификатор данных типа предмета в этой ячейке
        /// </summary>
        public string ItemTypeId { get => itemTypeId; private set => itemTypeId = value; }

        /// <summary>
        /// Количество предметов в ячейке
        /// </summary>
        public int ItemAmount { get => itemAmount; private set => itemAmount = value; }

        protected bool isEmpty;
        protected bool isDefault;
        
        protected string itemId;
        protected string itemTypeId;
        protected int itemAmount;

        /// <summary>
        /// Конструктор заполненной ячейки инвентаря
        /// </summary>
        /// <param name="itemId">Идентификатор данных предмета в этой ячейке</param>
        /// <param name="itemTypeId">Идентификатор типа данных предмета в этой ячейке</param>
        /// <param name="itemAmount">Количество предметов в ячейке</param>
        public InventoryCell(string itemId, string itemTypeId, int itemAmount = 1, bool isDefault = false, bool isEmpty = false)
        {
            if (String.IsNullOrEmpty(itemId))
            {
                ServiceDebug.LogError("Невалидный id предмета при создании ячейки инвентаря");
                itemId = String.Empty;
            }
            if (String.IsNullOrEmpty(itemTypeId))
            {
                ServiceDebug.LogError("Невалидный id типа предмета при создании ячейки инвентаря");
                itemTypeId = String.Empty;
            }
            
            if (itemAmount <= 0) itemAmount = 1;
            
            this.isEmpty = isEmpty;
            this.isDefault = isDefault;
            
            this.itemId = itemId;
            this.itemTypeId = itemTypeId;
            this.itemAmount = itemAmount;
        }

        #region Манипуляции количеством

        /// <summary>
        /// Добавить количество
        /// </summary>
        /// <param name="amount">Количество к добавлению</param>
        /// <returns>Излишек (если превысило максимум ячейки)</returns>
        public int IncreaseAmount(int amount, int maxAmount = DEFAULT_MAX_AMOUNT)
        {
            if (amount <= 0)
            {
                ServiceDebug.LogWarning($"Попытка добавления невалидного количества ({amount})");
                return 0;
            }

            int before = itemAmount;
            int target = before + amount;

            int excess = Mathf.Max(0, target - maxAmount);

            SetAmount(target, maxAmount);

            if (excess > 0)
            {
                onAmountExceeded?.Invoke(Id, excess);
            }

            return excess;
        }
        
        /// <summary>
        /// Убавить количество
        /// </summary>
        /// <param name="amount">Количество к убавлению</param>
        /// <returns>Остаток, который удалить не удалось (если в ячейке было меньше, чем удалялось)</returns>
        public int DecreaseAmount(int amount, int maxAmount = DEFAULT_MAX_AMOUNT)
        {
            if (amount <= 0)
            {
                ServiceDebug.LogWarning($"Попытка убавления невалидного количества ({amount})");
                return 0;
            }

            if (itemAmount <= 0)
            {
                return amount;
            }

            int before = itemAmount;
            int removed = Mathf.Min(before, amount);
            int target = before - removed;

            SetAmount(target, maxAmount);

            int residue = amount - removed;
            return residue;
        }

        #endregion
        
        #region Данные в ячейке

        /// <summary>
        /// Получить данные предмета в ячейке
        /// </summary>
        /// <param name="dataRegistry">Реестр данных однотипных предметов</param>
        /// <returns>Данные предмета</returns>
        public InventoryItem GetItemData(InventoryItemsRegistry dataRegistry)
        {
            if (isEmpty) return null;
            if (dataRegistry == null)
            {
                ServiceDebug.LogError("Реестр данных предметов не задан, данные не найдены");
                return null;
            }
            if (String.IsNullOrEmpty(itemId))
            {
                ServiceDebug.LogError("Невалидный id предмета ячейки инвентаря, данные не найдены");
                return null;
            }

            InventoryItem data = dataRegistry.GetById(itemId);
            return data;
        }

        /// <summary>
        /// Получить данные типа предмета в ячейке
        /// </summary>
        /// <param name="dataRegistry">Реестр типа данных однотипных предметов</param>
        /// <returns>Данные типа предмета</returns>
        public InventoryItemType GetTypeData(InventoryItemTypesRegistry typeRegistry)
        {
            if (isEmpty) return null;
            if (typeRegistry == null)
            {
                ServiceDebug.LogError("Реестр данных предметов не задан, данные не найдены");
                return null;
            }
            if (String.IsNullOrEmpty(itemTypeId))
            {
                ServiceDebug.LogError("Невалидный id типа предмета ячейки инвентаря, данные не найдены");
                return null;
            }

            InventoryItemType data = typeRegistry.GetById(itemTypeId);
            return data;
        }

        /// <summary>
        /// Сравнение содержимого двух ячеек (по свойствам)
        /// </summary>
        /// <param name="otherItemId">Идентификатор сравниваемого предмета</param>
        /// <param name="otherItemTypeId">Идентификатор типа сравниваемого предмета</param>
        /// <returns>true если предмет и тип совпадают, иначе false</returns>
        public bool IsContentSame(string otherItemId, string otherItemTypeId)
        {
            if (String.IsNullOrEmpty(otherItemId) || String.IsNullOrEmpty(otherItemTypeId))
            {
                ServiceDebug.LogError("Получены невалидные id, сравнение не выполнено");
                return false;
            }
            
            if (itemId == otherItemId && itemTypeId == otherItemTypeId)
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Сравнение содержимого двух ячеек
        /// </summary>
        /// <param name="otherCell">Сравниваемая (другая) ячейка</param>
        /// <returns>true если предмет и тип совпадают, иначе false</returns>
        public bool IsContentSame(InventoryCell otherCell)
        {
            if (String.IsNullOrEmpty(otherCell.ItemId) || String.IsNullOrEmpty(otherCell.ItemTypeId))
            {
                ServiceDebug.LogError("Получены невалидные id, сравнение не выполнено");
                return false;
            }
            
            if (itemId == otherCell.itemId && itemTypeId == otherCell.itemTypeId) // TODO добавить проверку на редкость
            {
                return true;
            }
            
            return false;
        }
        
        #endregion
        
        #region Internal

        private void SetAmount(int value, int maxAmount = DEFAULT_MAX_AMOUNT)
        {
            if (value < 0)
            {
                ServiceDebug.LogWarning("Количество не может быть менее 0, изменения не произведены");
                return;
            }

            int before = itemAmount;

            int clamped = value;
            if (clamped > maxAmount)
            {
                ServiceDebug.LogWarning($"Превышен максимум количества ячейки инвентаря, понижено с {clamped} до {maxAmount}");
                clamped = maxAmount;
            }

            if (before == clamped) return;

            itemAmount = clamped;

            onAmountChanged?.Invoke(Id, itemAmount);

            if (before > 0 && itemAmount == 0)
            {
                onAmountDepleted?.Invoke(Id);
            }
        }

        #endregion
    }
}