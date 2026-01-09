using System;
using UnityEngine;
using UnityEngine.UI;

namespace Extensions
{
    /// <summary>
    /// Абстракция кнопки
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class GenericButton : MonoBehaviour
    {
        /// <summary>
        /// Событие, вызываемое после клика кнопки
        /// </summary>
        public event Action onButtonClicked;
        
        protected Button button = default;

        protected virtual void Awake() => button = GetComponent<Button>();

        protected virtual void OnEnable() => button.onClick.AddListener(OnButtonAction);

        protected virtual void OnDisable() => button.onClick.RemoveListener(OnButtonAction);

        protected virtual void OnButtonAction()
        {
            OnButtonClick();
            onButtonClicked?.Invoke();
        }
        
        /// <summary>
        /// Код, выполняемый по клику кнопки
        /// </summary>
        public abstract void OnButtonClick();
    }
}
