using System.Collections.Generic;
using Extensions.Singleton;
using Unity.VisualScripting;
using UnityEngine;

namespace Extensions.UIWindows
{
    using ID;
    
    /// <summary>
    /// Контроллер окон пользовательского интерфейса
    /// </summary>
    public class UIWindowsController : MonoBehaviourSingleton<UIWindowsController>
    {
        /// <summary>
        /// Текущее окно в фокусе
        /// </summary>
        public UIWindow FocusedWindow => focusedWindow;
        
        [SerializeField]
        protected UIWindow startWindow = default;
        
        [SerializeField]
        protected List<UIWindow> preparedUIWindows =  new List<UIWindow>();
        
        [Header("Превью (назначаются автоматически)")]
        
        [SerializeField]
        protected List<UIWindow> openedUIWindows = new List<UIWindow>();
        
        [SerializeField]
        protected UIWindow focusedWindow = default;
        
        protected ID previousWindow = default;
        protected bool transitionInProgress = false;
        
        /// <summary>
        /// Открыть окно по идентификатору
        /// </summary>
        /// <param name="id"></param>
        public void OpenWindowByID(string id, bool setPrevious = false)
        {
            // Поиск окна в уже открытых
            foreach (UIWindow window in openedUIWindows)
            {
                if (window.Id.Id == id)
                {
                    previousWindow = focusedWindow.Id;

                    focusedWindow = window;
                    focusedWindow.gameObject.SetActive(true); 
                    
                    if (setPrevious)
                        focusedWindow.SetPreviousWindow(previousWindow);
                    
                    return;
                }
            }
            
            // Инстанцирование нового окна
            foreach (UIWindow window in preparedUIWindows)
            {
                if (window.Id.Id == id)
                {
                    OpenNewWindow(window);
                    return;
                }
            }
            
            Debug.LogError($"Window {id} not found");
        }
        
        /// <summary>
        /// Открытие предыдущего окна
        /// </summary>
        public void OpenPreviousWindow() => OpenWindowByID(focusedWindow.PreviousWindow.Id, false);

        /// <summary>
        /// Закрытие окна в фокусе
        /// </summary>
        public void CloseFocusedWindow() => focusedWindow.gameObject.SetActive(false);
        
        /// <summary>
        /// Закрытие предыдущего окна
        /// </summary>
        public void ClosePreviousWindow() => focusedWindow.PreviousWindow.GameObject().SetActive(false);

        private void OpenNewWindow(UIWindow window)
        {
            if (transitionInProgress)
                return;
            
            previousWindow = focusedWindow?.Id;
            focusedWindow = GameObject.Instantiate(window.gameObject, transform).GetComponent<UIWindow>();
            if (previousWindow)
                focusedWindow.SetPreviousWindow(previousWindow);
            openedUIWindows.Add(focusedWindow);
        }

        protected override void Awake()
        {
            base.Awake();
            Instance.preparedUIWindows = preparedUIWindows;
            
            OpenNewWindow(startWindow);
        }
    }
}