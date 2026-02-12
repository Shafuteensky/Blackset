using System;
using UnityEngine;
using Extensions.Singleton;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Глобальный координатор drag & drop инвентаря
    /// </summary>
    public class InventoryDragDropCoordinator : MonoBehaviourSingleton<InventoryDragDropCoordinator>
    {
        #region Events

        /// <summary>
        /// Начало драга
        /// </summary>
        public event Action<DragPayload> onDragStarted;

        /// <summary>
        /// Обновление позиции (для иконки-превью)
        /// </summary>
        public event Action<Vector2> onDragPositionChanged;

        /// <summary>
        /// Попытка дропа
        /// </summary>
        public event Action<DragPayload, object, string> onDropRequested;

        /// <summary>
        /// Завершение драга
        /// </summary>
        public event Action onDragEnded;

        #endregion

        public bool HasPayload => hasPayload;

        public DragPayload CurrentPayload => payload;

        private bool hasPayload;
        private DragPayload payload;

        #region Public API

        public void BeginDrag(string sourceInventoryId, string sourceCellId)
        {
            if (string.IsNullOrEmpty(sourceInventoryId) || string.IsNullOrEmpty(sourceCellId))
            {
                return;
            }

            payload = new DragPayload
            {
                SourceInventoryId = sourceInventoryId,
                SourceCellId = sourceCellId
            };

            hasPayload = true;

            onDragStarted?.Invoke(payload);
        }

        public void UpdatePosition(Vector2 screenPosition)
        {
            if (!hasPayload)
            {
                return;
            }

            onDragPositionChanged?.Invoke(screenPosition);
        }

        public void RequestDrop(object targetInventory, string targetCellId)
        {
            if (!hasPayload)
            {
                return;
            }

            onDropRequested?.Invoke(payload, targetInventory, targetCellId);
        }

        public void EndDrag()
        {
            if (!hasPayload)
            {
                return;
            }

            hasPayload = false;
            payload = default;

            onDragEnded?.Invoke();
        }

        #endregion
    }
}
