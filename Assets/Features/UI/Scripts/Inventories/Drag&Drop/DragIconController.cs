using Extensions.Generics;
using UnityEngine;
using Blackset.Inventories;

namespace Blackset.UI.InventoryManagement
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

        protected void InitializeElement(Inventory inventory, string cellId)
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