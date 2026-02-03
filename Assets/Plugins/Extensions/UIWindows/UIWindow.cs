using UnityEngine;
using Extensions.Identification;

namespace Extensions.UIWindows
{
    /// <summary>
    /// Окно интерфейса
    /// </summary>
    public sealed class UIWindow : MonoBehaviour
    {
        public UIWindowID Id => id;
        public UIWindowID PreviousWindow => previousWindow;
        
        [SerializeField]
        private UIWindowID id = default;

        private UIWindowID previousWindow = default;
        
        public void SetPreviousWindow(UIWindowID window) => previousWindow = window;
    }
}