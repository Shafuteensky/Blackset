using System;
using UnityEngine;
using UnityEngine.UI;

namespace Extensions.Generics
{
    /// <summary>
    /// Абстракция кнопки
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class AbstractButton : BaseAbstractButton
    {
        /// <summary>
        /// Событие, вызываемое после клика кнопки
        /// </summary>
        public event Action onButtonClicked;
        
        protected virtual void OnEnable() => button.onClick.AddListener(OnButtonAction);

        protected virtual void OnDisable() => button.onClick.RemoveListener(OnButtonAction);

        protected virtual void OnButtonAction()
        {
            OnButtonClick();
            onButtonClicked?.Invoke();
        }
    }
}
