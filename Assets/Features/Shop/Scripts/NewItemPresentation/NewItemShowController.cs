using System;
using Blackset.Data;
using Blackset.Data.Items.Types;
using Extensions.Log;
using Extensions.UIWindows;
using UnityEngine;

namespace Features.Shop
{
    /// <summary>
    /// Контроллер вывода информации о новом полученном предмете
    /// </summary>
    public class NewItemShowController : MonoBehaviour
    {
        [SerializeField]
        private UIWindowID windowID;

        private UIWindowsController uiWindowsController;
        
        private void Awake() => uiWindowsController = UIWindowsController.Instance;

        private void OnEnable() => BuyShopDiceButton.onItemBought += ShowNewItem;

        private void OnDisable() => BuyShopDiceButton.onItemBought -= ShowNewItem;

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
    }
}