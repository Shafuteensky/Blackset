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
    public class GenericDragIconController<TInventory, TItemCell, TData, TItemType> : InitializableMonoBehaviour
        where TInventory : BaseInventory<TItemCell, TData, TItemType>
        where TItemCell : BaseItemCell<TData, TItemType>
        where TData : EffectingItemData
        where TItemType : BaseItemType
    {
        [SerializeField]
        [Tooltip("Элемент UI")]
        protected GenericInventoryItemElement<TInventory, TItemCell, TData, TItemType> cellElement;
        
        protected GenericInventoryDragDropCoordinator<TInventory, TItemCell, TData, TItemType> dragDropCoordinator;

        protected virtual void Awake()
        {
            dragDropCoordinator = GenericInventoryDragDropCoordinator<TInventory, TItemCell, TData, TItemType>.Instance;
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

        protected void InitializeElement(TInventory inventory, string cellId)
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