using Blackset.Data.Items.Types;
using Blackset.Effects;
using Blackset.Inventory.Cells;
using Blackset.Inventory.Inventories;
using Extensions.Generics;
using UnityEngine;

namespace Blackset.UI.Inventory
{
    /// <summary>
    /// Абстракция контроллера иконки перетаскиваемого из инвентаря предмета
    /// </summary>
    public class DragIconController : InitializableMonoBehaviour
    {
        [SerializeField]
        [Tooltip("Элемент UI")]
        protected InventoryItemElement cellElement;
        
        protected InventoryDragDropCoordinator dragDropCoordinator;

        protected virtual void Awake()
        {
            dragDropCoordinator = InventoryDragDropCoordinator.Instance;
            Initialize(dragDropCoordinator != null);
        }
        
        protected virtual void OnEnable()
        {
            if (!IsInitialized) return;

            dragDropCoordinator.onDragStarted += InitializeElement; 
            dragDropCoordinator.onDragPositionChanged += UpdatePosition; 
            dragDropCoordinator.onDragEnded += FreeElement; 
            dragDropCoordinator.onDrop += FreeElement; 
        }
        
        protected virtual void OnDisable()
        {
            if (!IsInitialized) return;
            
            dragDropCoordinator.onDragStarted -= InitializeElement; 
            dragDropCoordinator.onDragPositionChanged -= UpdatePosition; 
            dragDropCoordinator.onDragEnded -= FreeElement; 
            dragDropCoordinator.onDrop -= FreeElement; 
        }

        #region UI Element

        protected void InitializeElement(Blackset.Inventory.Inventories.Inventory inventory, string cellId)
        {
            cellElement.CanvasGroup.blocksRaycasts = false;
            cellElement.CanvasGroup.alpha = 1;
            cellElement.InitializeElement(inventory, cellId);
        }
        
        protected void FreeElement()
        {
            cellElement.CanvasGroup.blocksRaycasts = false;
            cellElement.CanvasGroup.alpha = 0;
        }

        protected void UpdatePosition(Vector2 screenPosition)
        {
            cellElement.transform.position = screenPosition;
        }

        #endregion
    }
}