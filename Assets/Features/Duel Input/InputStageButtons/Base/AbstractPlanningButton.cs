using System;
using UnityEngine;
using UnityEngine.UI;

namespace Blackset.DecisionInput
{
    /// <summary>
    /// Базовая кнопка для окна планирования
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class AbstractPlanningButton : AbstractPlanningElement
    {
        /// <summary>
        /// Событие, вызываемое после клика кнопки
        /// </summary>
        public event Action onButtonClicked;

        protected Button button = default;

        protected virtual void Awake()
        {
            button = GetComponent<Button>();
            button.interactable = false;
        }

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

        protected override void SetInteractable(bool state)
        {
            if (button != null)
                button.interactable = state;
        }
    }
}