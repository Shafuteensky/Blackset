using System;
using Blackset.Data;
using Extensions.Data.InMemoryData;
using UnityEngine;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Extensions.Log;

namespace Blackset.Inventory
{
    /// <summary>
    /// Базовый предмет ячейки инвентаря
    /// </summary>
    [Serializable]
    public abstract class BaseItemCell<TData, TType> : InMemoryDataItem where TData : BaseData where TType : BaseItemType
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
        public int ItemAmount
        {
            get => itemAmount;
            set
            {
                if (value < 0)
                {
                    ServiceDebug.LogWarning("Количество не может быть менее 0, изменения не произведены");
                    return;
                }
                
                itemAmount = value;
                
                if (itemAmount > MAX_AMOUNT)
                {
                    int excess = itemAmount - MAX_AMOUNT;
                    itemAmount = MAX_AMOUNT;
                    
                    onAmountExceeded?.Invoke(Id, excess);
                    return;
                }
                
                if (itemAmount <= 0) onAmountDepleted?.Invoke(Id);
            }
        }

        [SerializeField]
        protected string itemId = string.Empty;
        [SerializeField]
        protected string itemTypeId = string.Empty;
        [SerializeField]
        protected int itemAmount;

        /// <summary>
        /// Конструктор ячейки инвентаря
        /// </summary>
        /// <param name="itemId">Идентификатор данных предмета в этой ячейке</param>
        /// <param name="itemTypeId">Идентификатор типа данных предмета в этой ячейке</param>
        /// <param name="itemAmount">Количество предметов в ячейке</param>
        public BaseItemCell(string itemId, string itemTypeId, int itemAmount = 1)
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
        public int Add(int amount)
        {
            if (amount <= 0)
            {
                ServiceDebug.LogWarning($"Попытка добавления невалидного количества ({amount})");
                return 0;
            }

            int freeSpace = MAX_AMOUNT - itemAmount;
            int added = Mathf.Min(freeSpace, amount);

            if (added > 0)
            {
                itemAmount += added;
                onAmountChanged?.Invoke(Id, itemAmount);
            }

            int excess = amount - added;
            return excess;
        }
        
        /// <summary>
        /// Убавить количество
        /// </summary>
        /// <param name="amount">Количество к убавлению</param>
        /// <returns>Остаток, который удалить не удалось (если в ячейке было меньше, чем удалялось)</returns>
        public int Remove(int amount)
        {
            if (amount <= 0)
            {
                ServiceDebug.LogWarning($"Попытка убавления невалидного количества ({amount})");
                return 0;
            }

            if (itemAmount <= 0)
            {
                onAmountDepleted?.Invoke(Id);
                return amount;
            }

            int removed = Mathf.Min(itemAmount, amount);
            itemAmount -= removed;

            onAmountChanged?.Invoke(Id, itemAmount);

            if (itemAmount == 0)
            {
                onAmountDepleted?.Invoke(Id);
            }

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
        public TData GetData(BaseDataRegistry<TData> dataRegistry)
        {
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
        public TType GetType(BaseDataRegistry<TType> typeRegistry)
        {
            if (String.IsNullOrEmpty(itemTypeId))
            {
                ServiceDebug.LogError("Невалидный id типа предмета ячейки инвентаря, данные не найдены");
                return null;
            }

            TType data = typeRegistry.GetById(itemTypeId);
            return data;
        }

        /// <summary>
        /// Сравнение двух предметов
        /// </summary>
        /// <param name="otherItemId">Идентификатор сравниваемого предмета</param>
        /// <param name="otherItemTypeId">Идентификатор типа сравниваемого предмета</param>
        /// <returns>true если предмет и тип совпадают, иначе false</returns>
        public bool IsSame(string otherItemId, string otherItemTypeId)
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
        
        #endregion
        
    }
}