using System;
using Blackset.Data.Registries;
using Extensions.Data.InMemoryData;
using UnityEngine;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Extensions.Log;

namespace Blackset.Inventory.Cells
{
    /// <summary>
    /// Базовый предмет ячейки инвентаря
    /// </summary>
    /// <typeparam name="TData">Тип данных предмета</typeparam>
    /// <typeparam name="TType">Тип типа предмета</typeparam>
    [Serializable]
    public abstract class BaseItemCell<TData, TType> : InMemoryDataItem 
        where TData : BaseData 
        where TType : BaseItemType
    {
        /// <summary>
        /// Максимальное количество предмета в ячейке
        /// </summary>
        public const int MAX_AMOUNT = 99;

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
        public bool IsEmpty => isEmpty;
        /// <summary>
        /// Является ли ячейка дефолтной
        /// </summary>
        public bool IsDefault => isDefault;
        
        /// <summary>
        /// Идентификатор данных предмета в этой ячейке
        /// </summary>
        public string ItemId => itemId;
        /// <summary>
        /// Идентификатор данных типа предмета в этой ячейке
        /// </summary>
        public string ItemTypeId => itemTypeId;

        /// <summary>
        /// Количество предметов в ячейке
        /// </summary>
        public int ItemAmount => itemAmount;

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
        protected BaseItemCell(string itemId, string itemTypeId, int itemAmount = 1, bool isDefault = false, bool isEmpty = false)
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
        public int IncreaseAmount(int amount)
        {
            if (amount <= 0)
            {
                ServiceDebug.LogWarning($"Попытка добавления невалидного количества ({amount})");
                return 0;
            }

            int before = itemAmount;
            int target = before + amount;

            int excess = Mathf.Max(0, target - MAX_AMOUNT);

            SetAmount(target);

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
        public int DecreaseAmount(int amount)
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

            SetAmount(target);

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
        public TData GetItemData(BaseDataRegistry<TData> dataRegistry)
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

            TData data = dataRegistry.GetById(itemId);
            return data;
        }

        /// <summary>
        /// Получить данные типа предмета в ячейке
        /// </summary>
        /// <param name="dataRegistry">Реестр типа данных однотипных предметов</param>
        /// <returns>Данные типа предмета</returns>
        public TType GetTypeData(BaseDataRegistry<TType> typeRegistry)
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

            TType data = typeRegistry.GetById(itemTypeId);
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
        public bool IsContentSame(BaseItemCell<TData, TType> otherCell)
        {
            if (String.IsNullOrEmpty(otherCell.ItemId) || String.IsNullOrEmpty(otherCell.ItemTypeId))
            {
                ServiceDebug.LogError("Получены невалидные id, сравнение не выполнено");
                return false;
            }
            
            if (itemId == otherCell.itemId && itemTypeId == otherCell.itemTypeId)
            {
                return true;
            }
            
            return false;
        }
        
        #endregion
        
        #region Internal

        private void SetAmount(int value)
        {
            if (value < 0)
            {
                ServiceDebug.LogWarning("Количество не может быть менее 0, изменения не произведены");
                return;
            }

            int before = itemAmount;

            int clamped = value;
            if (clamped > MAX_AMOUNT) clamped = MAX_AMOUNT;

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