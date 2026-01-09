using UnityEngine;

namespace Extensions.UIWindows
{
    using ID;
    
    /// <summary>
    /// Окно интерфейса
    /// </summary>
    public class UIWindow : MonoBehaviour
    {
        public ID Id => id;
        public ID PreviousWindow => previousWindow;
        
        [SerializeField]
        protected ID id = default;

        protected ID previousWindow = default;
        
        public virtual void SetPreviousWindow(ID window) => previousWindow = window;
    }
}