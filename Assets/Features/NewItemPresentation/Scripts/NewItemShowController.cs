using Blackset.Data.Registries;
using Extensions.Generics;
using Extensions.Log;
using Extensions.UIWindows;
using UnityEngine;
using Blackset.Inventories;
using Blackset.Inventories.Cells;

namespace Blackset.Shop
{
    /// <summary>
    /// Контроллер вывода информации о новом полученном предмете
    /// </summary>
    public class NewItemShowController : InitializableMonoBehaviour
    {
        [Header("Окно показа новых предметов"), Space]
        [SerializeField] private UIWindowID windowID;
        
        private UIWindowsController uiWindowsController;
        
        private Inventory playerInventory;
        
        private void Awake()
        {
            uiWindowsController = UIWindowsController.Instance;
            playerInventory = GameData.Instance.PlayerDataFacade.Inventory;
            Initialize(uiWindowsController != null && windowID != null && playerInventory != null);
        }

        private void OnEnable()
        {
            playerInventory.onCellAdded += RememberNewItem;
        }

        private void OnDisable()
        {
            playerInventory.onCellAdded -= RememberNewItem;
        }

        #region Учет новых добавленных предметов

        private void RememberNewItem(InventoryCell cell)
        {
            GameData.Instance.NewItemsPresenterInventory.Add(cell);
        }

        #endregion
        
        #region Показ новых предметов
        
        private void ShowNewItem()
        {
            OpenPresentationWindow();
        }

        private void OpenPresentationWindow()
        {
            if (windowID == null)
            {
                ServiceDebug.LogError("Идентификатор окна не определен");
                return;
            }
            uiWindowsController.OpenWindowByID(windowID.Id);
        }
        
        #endregion
    }
}