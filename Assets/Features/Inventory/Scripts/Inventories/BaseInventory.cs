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
    public abstract class BaseInventory<TItemCell, TData, TType> : InMemoryDataContainer<TItemCell>
        where TItemCell : BaseItemCell<TData, TType>
        where TData : BaseData
        where TType : BaseItemType
    {
        /// <summary>
        /// Реестр данных предметов для разрешения itemId -> данные
        /// </summary>
        public BaseDataRegistry<TData> DataRegistry => dataRegistry;
        /// <summary>
        /// Реестр данных предметов для разрешения itemId -> данные
        /// </summary>
        public BaseDataRegistry<TType> TypeRegistry => typeRegistry;
        
        [Header("Реестры игровых данных"), Space]
        [SerializeField]
        protected BaseDataRegistry<TData> dataRegistry;
        [SerializeField]
        protected BaseDataRegistry<TType> typeRegistry;

        #region Получение данных ячеек
        
        /// <summary>
        /// Получить данные предмета определенной ячейки
        /// </summary>
        /// <param name="item">Ячейка предмета</param>
        public TData GetCellItemData(TItemCell itemCell)
        {
            if (!IsReferencesValid(itemCell, dataRegistry)) return null;
            return itemCell.GetItemData(dataRegistry);
        }
        
        /// <summary>
        /// Получить данные типа предмета определенной ячейки
        /// </summary>
        /// <param name="item">Ячейка предмета</param>
        public TType GetCellTypeData(TItemCell itemCell)
        {
            if (!IsReferencesValid(itemCell, typeRegistry)) return null;
            return itemCell.GetTypeData(typeRegistry);
        }

        #endregion

        #region Internal

        private bool IsReferencesValid(TItemCell itemCell, BaseData registry)
        {
            if (itemCell == null)
            {
                ServiceDebug.LogError("Получены невалидные данные ячейки, данные не найдены");
                return false;
            }
            if (registry == null)
            {
                ServiceDebug.LogError("Реестр данных не назначен, данные не найдены");
                return false;
            }
            
            return true;
        }
        
        #endregion
        
    }
}