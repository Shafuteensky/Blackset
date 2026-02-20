using System;
using Blackset.Inventories;
using Blackset.Inventories.Cells;
using Extensions.Log;
using UnityEngine;
using Extensions.Singleton;

namespace Blackset.UI.InventoryManagement
{
    /// <summary>
    /// Базовый скрипт координатора drag & drop инвентаря определенного типа хранимых данных
    /// </summary>
    public class InventoryDragDropCoordinator : 
        MonoBehaviourSingleton<InventoryDragDropCoordinator>
    {
        #region Events

        /// <summary>
        /// Начало драга
        /// </summary>
        /// <param name="TInventory">Инвентарь</param>
        /// <param name="string">Идентификатор ячейки</param>
        public event Action<Inventory, string> onDragStarted;
        /// <summary>
        /// Завершение драга
        /// </summary>
        public event Action onDragEnded;
        /// <summary>
        /// Дроп 
        /// </summary>
        public event Action onDrop;
        /// <summary>
        /// Попытка дропа
        /// </summary>
        /// <param name="TInventory">Инвентарь</param>
        /// <param name="string">Идентификатор ячейки</param>
        public event Action<Inventory, string> onDropRequested;

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
        private Inventory sourceInventory;
        private string sourceCellId;

        #region Drag&Drop
        
        /// <summary>
        /// Начать перенос ячейки инвентаря
        /// </summary>
        /// <param name="fromInventory">Инвентарь, из которого начинается перетаскивание</param>
        /// <param name="fromCellId">Идентификатор ячейки инвентаря, из которой начинается перетаскивание</param>
        public void BeginDrag(Inventory fromInventory, string fromCellId)
        {
            if (fromInventory == null) return;
            if (IsAbleToDrag(fromInventory.GetById(fromCellId)))
            {
                InitializeData(fromInventory, fromCellId);
                onDragStarted?.Invoke(sourceInventory, fromCellId);
            }
        }

        /// <summary>
        /// Завершить перенос ячейки инвентаря
        /// </summary>
        public void EndDrag()
        {
            ClearData();
            onDragEnded?.Invoke();
        }

        /// <summary>
        /// Запрос перетаскивания в ячейку инвентаря
        /// </summary>
        /// <param name="targetInventory">Инвентарь, из которого происходит дроп</param>
        /// <param name="targetCellId">Идентификатор ячейки инвентаря, из которой происходит дроп</param>
        public void RequestDrop(Inventory targetInventory, string targetCellId = null)
        {
            if (!hasPayload || targetInventory == null || sourceInventory == null) return;
            if (String.IsNullOrEmpty(targetCellId)) targetCellId = String.Empty; // На случай если перенос не в определенную ячейку, а просто в инвентарь

            DropItem(targetInventory, targetCellId);
            onDropRequested?.Invoke(targetInventory, targetCellId);
            onDrop?.Invoke();
            ClearData();
        }
        
        /// <summary>
        /// Обновить позицию курсора переноса
        /// </summary>
        /// <param name="screenPosition">Экранная позиция курсора</param>
        public void UpdatePosition(Vector2 screenPosition)
        {
            if (!hasPayload) return;

            onDragPositionChanged?.Invoke(screenPosition);
        }

        #endregion
        
        #region Данные о перетаскиваемой ячейке

        private void InitializeData(Inventory fromInventory, string fromCellId)
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
        /// <param name="targetInventory">Инвентарь, из которого происходит пермещение</param>
        /// <param name="targetCellId">Идентификатор перемещаемой ячейки</param>
        private void DropItem(Inventory targetInventory, string targetCellId = null)
        {
            if (targetInventory == null)
            {
                ServiceDebug.LogError("Ссылка на инвентарь отсутствует, drop не выполнен");
                return;
            }
            
            InventoryCell sourceCell = sourceInventory.GetById(sourceCellId);
            InventoryCell targetCell = null;
            if ( !String.IsNullOrEmpty(targetCellId) ) targetCell = targetInventory.GetById(targetCellId);

            if (sourceCell == null)
            {
                ServiceDebug.LogError("Ячейка исходных данных не найдена, drop не выполнен");
                return;
            }
            
            if ( !hasPayload || ReferenceEquals(targetCell, sourceCell) ) return;
            if ( sourceCell.IsDefault || sourceCell.IsEmpty ) return;
            
            // Перемещение в определенную ячейку
            if ( !String.IsNullOrEmpty(targetCellId) )
                sourceInventory.MoveItem(sourceCellId, targetInventory, targetCellId);
            // Перемещение в любую ячейку
            else
                sourceInventory.MoveItem(sourceCellId, targetInventory); 
        }

        private bool IsAbleToDrag(InventoryCell cell)
        {
            if (cell.IsDefault || cell.IsEmpty) return false;
            return true;
        }

        #endregion
    }
}
