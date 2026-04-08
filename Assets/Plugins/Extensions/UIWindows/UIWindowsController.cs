using System.Collections.Generic;
using Extensions.Log;
using Extensions.Singleton;
using Unity.VisualScripting;
using UnityEngine;

namespace Extensions.UIWindows
{
    /// <summary>
    /// Контроллер окон пользовательского интерфейса
    /// </summary>
    public class UIWindowsController : MonoBehaviourSingleton<UIWindowsController>
    {
        #region Свойства
        
        /// <summary>
        /// Текущее окно в фокусе
        /// </summary>
        public UIWindow FocusedWindow => focusedWindow;
        
        #endregion
        
        #region Параметры
        
        [Header("Настройки"), Space]
        [SerializeField]
        protected UIWindow startWindow;
        [SerializeField]
        protected List<UIWindow> preparedUIWindows =  new List<UIWindow>();
        [SerializeField]
        protected Transform root;
        
        [Header("Превью (назначаются автоматически)"), Space]
        [SerializeField]
        protected List<UIWindow> openedUIWindows = new List<UIWindow>();
        [SerializeField]
        protected UIWindow focusedWindow;
        
        #endregion
        
        #region Переменные
        
        protected UIWindowID previousWindow;
        protected bool transitionInProgress = false;
        
        #endregion

        #region MonoBehaviour
        
        protected override void Awake()
        {
            base.Awake();
            Instance.preparedUIWindows = preparedUIWindows;
            
            if (root == null) root = transform;
            OpenNewWindow(startWindow);
        }
        
        #endregion

        #region Управление окнами
        
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
                    focusedWindow.transform.SetAsLastSibling();
                    
                    if (setPrevious) focusedWindow.SetPreviousWindow(previousWindow);
                    
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
            
            ServiceDebug.LogError($"Окно с id {id} не найдено в {nameof(UIWindowsController)}");
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
            if (transitionInProgress) return;
            
            previousWindow = focusedWindow?.Id;
            focusedWindow = Instantiate(window.gameObject, root).GetComponent<UIWindow>();
            
            if (previousWindow) focusedWindow.SetPreviousWindow(previousWindow);
            
            focusedWindow.transform.SetAsLastSibling();
            openedUIWindows.Add(focusedWindow);
        }
        
        #endregion
    }
}