using System;
using Blackset.Data.Base;
using Blackset.Data.Items.Types;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;
using UnityEngine;
using Extensions.Singleton;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Базовый скрипт координатора drag & drop инвентаря определенного типа хранимых данных
    /// </summary>
    public abstract class GenericInventoryDragDropCoordinator<TInventory, TItemCell, TData, TItemType> : 
        MonoBehaviourSingleton<GenericInventoryDragDropCoordinator<TInventory, TItemCell, TData, TItemType>>
        where TInventory : BaseInventory<TItemCell, TData, TItemType>
        where TItemCell : BaseItemCell<TData, TItemType>
        where TData : BaseData
        where TItemType : BaseItemType
    {
        #region Events

        /// <summary>
        /// Начало драга
        /// </summary>
        /// <param name="TInventory">Инвентарь</param>
        /// <param name="string">Идентификатор ячейки</param>
        public event Action<TInventory, string> onDragStarted;
        /// <summary>
        /// Завершение драга
        /// </summary>
        public event Action onDragEnded;
        /// <summary>
        /// Попытка дропа
        /// </summary>
        /// <param name="TInventory">Инвентарь</param>
        /// <param name="string">Идентификатор ячейки</param>
        public event Action<TInventory, string> onDropRequested;

        /// <summary>
        /// Обновление позиции курсора переноса (для иконки-превью)
        /// </summary>
        public event Action<Vector2> onDragPositionChanged;
        
        #endregion

        /// <summary>
        /// Переданы ли данные об инвентаре и ячейке (в процессе ли перетаскивание)
        /// </summary>
        public bool HasPayload => hasPayload;

        private bool hasPayload;
        private TInventory sourceInventory;
        private string sourceCellId;

        /// <summary>
        /// Начать перенос ячейки инвентаря
        /// </summary>
        /// <param name="fromInventory">Инвентарь, из которого идет перетаскивание</param>
        /// <param name="sourceCellId">Ячейка инвентаря, которая в процессе перетаскивания</param>
        public void BeginDrag(TInventory fromInventory, string fromCellId)
        {
            if (fromInventory == null) return;
            IsAbleToDrag(fromInventory.GetById(fromCellId));
            
            InitializeData(fromInventory, fromCellId);
            
            onDragStarted?.Invoke(sourceInventory, fromCellId);
        }

        /// <summary>
        /// Обновить позицию курсора переноса
        /// </summary>
        /// <param name="screenPosition">Экранная позиция курсора</param>
        public void UpdatePosition(Vector2 screenPosition)
        {
            if (!hasPayload)
            {
                return;
            }

            onDragPositionChanged?.Invoke(screenPosition);
        }

        /// <summary>
        /// Запроса перетаскивания в ячейку инвентаря
        /// </summary>
        /// <param name="targetInventory"></param>
        /// <param name="targetCellId"></param>
        public void RequestDrop(TInventory targetInventory, string targetCellId)
        {
            if (!hasPayload || targetInventory == null || sourceInventory == null) return;
            if (targetCellId == null) targetCellId = String.Empty; // На случай если перенос не в определенную ячейку, а просто в инвентарь

            DropItem(targetInventory, targetCellId);
            onDropRequested?.Invoke(targetInventory, targetCellId);
            ClearData();
        }

        /// <summary>
        /// Завершить перенос ячейки инвентаря
        /// </summary>
        public void EndDrag()
        {
            if (!hasPayload) return;

            ClearData();
            onDragEnded?.Invoke();
        }

        #region Данные о перетаскиваемой ячейке

        private void InitializeData(TInventory fromInventory, string fromCellId)
        {
            if (fromInventory == null || string.IsNullOrEmpty(fromCellId))
            {
                ClearData();
                return;
            }

            sourceInventory = fromInventory;
            sourceCellId = fromCellId;
            hasPayload = true;
        }

        private void ClearData()
        {
            sourceInventory = null;
            sourceCellId = String.Empty;
            
            hasPayload = false;
        }
        
        #endregion

        #region Работа с инвентарями

        /// <summary>
        /// Переместить в эту ячейку предмет из другой ячейки
        /// </summary>
        /// <param name="fromCellId">Идентификатор перемещаемой ячейки</param>
        /// <param name="fromInventory">Инвентарь, из которого происходит пермещение</param>
        private void DropItem(TInventory fromInventory, string fromCellId)
        {
            TItemCell sourceCell = sourceInventory.GetById(sourceCellId);
            TItemCell fromCell = fromInventory.GetById(fromCellId);

            if (!hasPayload || sourceCell == null || sourceCell == fromCell) return;
            if (fromCell.IsDefault || fromCell.IsEmpty) return;
            
            // Перемещение в определенную ячейку
            if (fromCellId != String.Empty)
            {
                if (fromCell == null) return;
                
                if (fromCell.IsContentSame(sourceCell))
                {
                    int residue = sourceInventory.AddItem(fromCell, true, fromInventory.GetIndexById(fromCellId));
                    
                    // Если перемещено все — удалить 
                    if (residue == 0)
                        fromInventory.RemoveItem(fromCellId);
                    // Если не все перемещено — просто убавить
                    else
                    {
                        int added = sourceCell.ItemAmount - residue;
                        fromInventory.RemoveItem(fromCellId, added);
                    }
                }
                else
                {
                    if (fromInventory == sourceInventory)
                        sourceInventory.SwapItem(sourceCellId, fromCellId);
                    else
                        fromInventory.MoveItem(fromCellId, sourceInventory, sourceCellId);
                }
            }
            // Перемещение в любую ячейку
            else
                fromInventory.MoveItem(fromCellId, sourceInventory); 
        }

        private bool IsAbleToDrag(TItemCell cell)
        {
            if (cell.IsDefault || cell.IsEmpty) return false;
            return true;
        }

        #endregion
    }
}
