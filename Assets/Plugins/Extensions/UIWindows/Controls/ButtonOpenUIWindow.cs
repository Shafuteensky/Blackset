using Extensions.Logs;
using UnityEngine;

namespace Extensions.UIWindows
{
    using ID;
    
    /// <summary>
    /// Кнопка для открытия окна интерфейса
    /// </summary>
    public class ButtonOpenUIWindow : GenericButton
    {
        [SerializeField] 
        protected bool needToCloseFocused = true;
        
        [SerializeField] 
        protected ID UIWindowToOpen = default;
        
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