using System.Collections.Generic;
using Extensions.Generics;
using Extensions.Log;
using Extensions.UIWindows;
using UnityEngine;

namespace Features.Shop
{
    /// <summary>
    /// Контроллер вывода информации о новом полученном предмете
    /// </summary>
    public class NewItemShowController : InitializableMonoBehaviour
    {
        [Header("Окно показа новых предметов"), Space]
        [SerializeField]
        private UIWindowID windowID;
        
        [Header("Инвентари игрока"), Space]
        [SerializeField]
        private Blackset.Inventory.Inventories.Inventory playerDicesInventory;
        [SerializeField]
        private Blackset.Inventory.Inventories.Inventory playerConsumablesInventory;

        private UIWindowsController uiWindowsController;
        
        private List<string> addedDices = new();
        private List<string> addedConsumables = new();
        
        private void Awake()
        {
            uiWindowsController = UIWindowsController.Instance;
            Initialize(uiWindowsController != null && windowID != null && 
                       playerDicesInventory != null && playerConsumablesInventory != null);
        }

        private void OnEnable()
        {
            playerDicesInventory.onCellAdded += RememberNewDice;
            playerConsumablesInventory.onCellAdded += RememberNewConsumables;
        }

        private void OnDisable()
        {
            playerDicesInventory.onCellAdded -= RememberNewDice;
            playerConsumablesInventory.onCellAdded -= RememberNewConsumables;
        }

        #region Учет новых добавленных предметов

        private void RememberNewDice(string cellId)
        {
            addedDices.Add(cellId);
        }

        private void RememberNewConsumables(string cellId)
        {
            addedConsumables.Add(cellId);
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