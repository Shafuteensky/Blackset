using Extensions.Generics;
using Extensions.Log;
using UnityEngine;

namespace Extensions.UIWindows
{
    /// <summary>
    /// Кнопка для открытия окна интерфейса
    /// </summary>
    public class ButtonOpenUIWindow : AbstractButton
    {
        [SerializeField] 
        protected bool needToCloseFocused = true;
        
        [SerializeField] 
        protected UIWindowID UIWindowToOpen = default;
        
        /// <summary>
        /// Открытие нового окна по нажатию на кнопку
        /// </summary>
        public override void OnButtonClick()
        {
            if (!UIWindowToOpen)
            {
                ServiceDebug.LogError($"{nameof(UIWindowToOpen)} is null");
                return;
            }
            
            UIWindowsController windowsController = UIWindowsController.Instance;

            if (needToCloseFocused) 
                windowsController.CloseFocusedWindow();
            windowsController.OpenWindowByID(UIWindowToOpen.Id);
        }
    }
}