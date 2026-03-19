using Extensions.Log;
using Extensions.UIWindows;
using UnityEngine;

namespace Blackset.Shop
{
    /// <summary>
    /// Контроллер окна презентации новых полученных предметов
    /// </summary>
    /// <remarks>
    /// Выводит окно показа новых предметов, если презентационный инвентарь не пуст
    /// </remarks>
    public class NewItemsWindowController : AbstractNewItemsShowController
    {
        protected UIWindowsController uiWindowsController;
        
        [Header("Окно показа новых предметов"), Space]
        [SerializeField] private UIWindowID windowID;

        protected override void Awake()
        {
            base.Awake();
            
            uiWindowsController = UIWindowsController.Instance;
        }
        
        protected override void OnNewItemsShowRequested() => OpenPresentationWindow();
        
        private void OpenPresentationWindow()
        {
            if (windowID == null)
            {
                ServiceDebug.LogError("Идентификатор окна не определен, окно показа новых предметов не открыто");
                return;
            }
            uiWindowsController.OpenWindowByID(windowID.Id);
        }
    }
}